using App.Application.Handlers.GeneralLedger.JournalEntry;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.SafesBanksFundService
{
    public class SafesBanksFundServiceHandler : IRequestHandler<SafesBanksFundServiceRequest, bool>
    {
        private readonly IRepositoryQuery<GLBank> _gLBankQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryCommand<GlReciepts> _receiptCommand;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _gLJournalEntryQuery;
        private readonly IMediator _mediator;

        public SafesBanksFundServiceHandler(IRepositoryQuery<GLBank> gLBankQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryCommand<GlReciepts> receiptCommand, IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery, IMediator mediator, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery)
        {
            _gLBankQuery = gLBankQuery;
            _gLSafeQuery = gLSafeQuery;
            _glRecieptsQuery = glRecieptsQuery;
            _receiptCommand = receiptCommand;
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            _mediator = mediator;
            _gLJournalEntryQuery = gLJournalEntryQuery;
        }
        public async Task<bool> Handle(SafesBanksFundServiceRequest request, CancellationToken cancellationToken)
        {

            //Reciepts
            int FA_Id = 0;
            string recNoteAr = "";
            string recNoteEn = "";
            double amount = 0;
            int ParentTypeId = 0;
            int RecieptTypeId = 0;
            if (request.table.IsBank)
            {
                FA_Id = _gLBankQuery.TableNoTracking.Where(x => x.Id == request.table.BankId).FirstOrDefault().FinancialAccountId ?? 0;
                recNoteAr = "ارصدة اول المدة البنوك" + "-" + request.table.Code.ToString();
                recNoteEn = "Entry Fund Bank" + "-" + request.table.Code.ToString();
            }
            else
            {
                FA_Id = _gLSafeQuery.TableNoTracking.Where(x => x.Id == request.table.SafeId).FirstOrDefault().FinancialAccountId ?? 0;
                recNoteAr = "ارصدة اول المدة الخزائن" + "-" + request.table.Code.ToString();
                recNoteEn = "Entry Fund Safe" + "-" + request.table.Code.ToString();
            }

            var rec = new List<GlReciepts>();
            foreach (var item in request.fundsDetailsList)
            {

                ParentTypeId = request.table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds;
                amount = item.Creditor - item.Debtor;
                if (request.table.IsBank)
                {
                    if (amount < 0)
                        RecieptTypeId = (int)DocumentType.BankCash;
                    else if (amount > 0)
                        RecieptTypeId = (int)DocumentType.BankPayment;
                }
                else if (!request.table.IsBank)
                {
                    if (amount < 0)
                        RecieptTypeId = (int)DocumentType.SafeCash;
                    else if (amount > 0)
                        RecieptTypeId = (int)DocumentType.SafePayment;
                }


                if (request.isUpdate)
                {
                    await deleteRecAndJournalEntry(new int[] { request.table.DocumentId }, ParentTypeId);

                }

                var rec2 = new GlReciepts()
                {
                    Amount = amount,
                    Creditor = item.Creditor,
                    Debtor = item.Debtor,
                    PaymentMethodId = item.PaymentId,
                    BenefitId = request.table.IsBank ? request.table.BankId ?? 0 : request.table.SafeId ?? 0,
                    Code = request.table.Code, // find
                    FinancialAccountId = FA_Id,
                    IsAccredit = false,
                    Notes = "",
                    CreationDate = DateTime.Now,
                    BranchId = request.userInfo.CurrentbranchId,
                    SafeID = request.table.SafeId,
                    BankId = request.table.BankId,
                    ParentTypeId = ParentTypeId,
                    Authority = (int)AuthorityTypes.DirectAccounts,
                    NoteAR = recNoteAr,
                    NoteEN = recNoteEn,
                    RecieptDate = request.table.DocDate,
                    UserId = request.userInfo.userId,
                    RecieptTypeId = RecieptTypeId,     //find
                    Signal = amount < 0 ? 1 : -1,
                    RecieptType = request.table.Code.ToString(), // find
                    ChequeNumber = "",
                    Serialize = 0,
                    EntryFundId = request.table.DocumentId
                };
                rec.Add(rec2);
            }

            _receiptCommand.AddRange(rec);
            bool saved = await _receiptCommand.SaveAsync();
            if (saved)
            {
                var GLSettings = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => x.branchId == request.userInfo.CurrentbranchId).Where(x => x.MainType == 4 && (request.table.IsBank ? x.RecieptsType == 29 : x.RecieptsType == 28));
                int Fund_FAId = GLSettings.OrderBy(x => x.Id).LastOrDefault().FinancialAccountId ?? 0;
                var entryDetails = new List<JournalEntryDetail>();
                var totalAmount = request.fundsDetailsList.Sum(x => x.Creditor - x.Debtor);

                entryDetails.AddRange(new[]
                {
                    new JournalEntryDetail
                    {
                        Credit = totalAmount < 0 ? Math.Abs(totalAmount) : 0,
                        Debit = totalAmount > 0 ? Math.Abs(totalAmount) : 0,
                        DescriptionAr = recNoteAr,
                        DescriptionEn = recNoteEn,
                        FinancialAccountId = Fund_FAId,
                    },
                    new JournalEntryDetail
                    {
                        Credit = totalAmount > 0 ? Math.Abs(totalAmount) : 0,
                        Debit = totalAmount < 0 ? Math.Abs(totalAmount) : 0,
                        DescriptionAr = recNoteAr,
                        DescriptionEn = recNoteEn,
                        FinancialAccountId =FA_Id,
                    }

                });

                var note = request.table.IsBank ? $"ارصدة اول المدة بنوك {request.table.Code}" : $"ارصدة اول المدة خزائن {request.table.Code}";
                if (!request.isUpdate)
                {

                    await _mediator.Send(new AddJournalEntryRequest
                    {
                        BranchId = request.userInfo.CurrentbranchId,
                        FTDate = request.table.DocDate,
                        InvoiceId = request.table.DocumentId,
                        DocType = request.table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds,
                        isAuto = true,
                        IsAccredit = true,
                        JournalEntryDetails = entryDetails,
                        Notes = note,
                    });
                    //await _journalEntryBusiness.AddJournalEntry(new JournalEntryParameter()
                    //{
                    //    BranchId = userInfo.CurrentbranchId,
                    //    FTDate = table.DocDate,
                    //    InvoiceId = table.DocumentId,
                    //    DocType = table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds,
                    //    isAuto = true,
                    //    IsAccredit = true,
                    //    JournalEntryDetails = entryDetails,
                    //    Notes = note,
                    //});
                }
                else
                {
                    var jeId = _gLJournalEntryQuery.TableNoTracking.Where(x => x.InvoiceId == request.table.DocumentId && x.DocType == (request.table.IsBank ? (int)DocumentType.BankFunds : (int)DocumentType.SafeFunds)).FirstOrDefault()?.Id ?? 0;
                    await _mediator.Send(new UpdateJournalEntryRequest
                    {
                        BranchId = request.userInfo.CurrentbranchId,
                        FTDate = request.table.DocDate,
                        fromSystem = true,
                        IsAccredit = true,
                        Notes = note,
                        journalEntryDetails = entryDetails,
                        Id = jeId
                    });
  
                }

            }
            return true;
        }
        public async Task deleteRecAndJournalEntry(int[] Ids, int DocType, bool fromDeleteApi = false)
        {
            var findRecs = await _glRecieptsQuery.GetAllAsyn(x => Ids.Contains(x.EntryFundId ?? 0));
            if (findRecs.Any())
            {
                if (!fromDeleteApi)
                {
                    _receiptCommand.RemoveRange(findRecs);
                    var RecDeleted = await _receiptCommand.SaveAsync();
                }
                else
                {
                    findRecs.ToList().ForEach(x => x.IsBlock = true);
                    await _receiptCommand.UpdateAsyn(findRecs);
                }
            }
        }
    }
}
