using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Handlers.MultiCollectionReceipts.AddMultiCollectionReceipts;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Infrastructure;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using System.IO;
using System.Threading;

namespace App.Application.Handlers.MultiCollectionReceipts.EditMultiCollectionReceipts
{
    public class EditMultiCollectionReceiptsHandler : IRequestHandler<EditMultiCollectionReceiptsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryCommand<GlReciepts> _GlRecieptsCommand;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryCommand<InvoiceMaster> _InvoiceMasterCommand;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsQuery;
        private readonly IRepositoryCommand<InvoicePaymentsMethods> _InvoicePaymentsMethodsCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IMediator _mediator;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryRepositoryCommand;
        private readonly IRoundNumbers _roundNumbers;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IFilesOfInvoices filesOfInvoices;
        private readonly IHistoryReceiptsService ReceiptsHistory;
        private readonly IRepositoryQuery<GLJournalEntry> _GLJournalEntryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> _GLJournalEntryDetailsCommand;
        public EditMultiCollectionReceiptsHandler(IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryCommand<GlReciepts> glRecieptsCommand, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, iUserInformation iUserInformation, IMediator mediator, IHistoryInvoiceService historyInvoiceService, IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryRepositoryCommand, IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentsMethodsQuery, IRepositoryCommand<InvoicePaymentsMethods> invoicePaymentsMethodsCommand, IRepositoryCommand<InvoiceMaster> invoiceMasterCommand, IRoundNumbers roundNumbers, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IFilesOfInvoices filesOfInvoices, IHistoryReceiptsService receiptsHistory, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery, IRepositoryCommand<GLJournalEntryDetails> gLJournalEntryDetailsCommand)
        {
            _GlRecieptsQuery = glRecieptsQuery;
            _GlRecieptsCommand = glRecieptsCommand;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _mediator = mediator;
            HistoryInvoiceService = historyInvoiceService;
            InvoiceMasterHistoryRepositoryCommand = invoiceMasterHistoryRepositoryCommand;
            _InvoicePaymentsMethodsQuery = invoicePaymentsMethodsQuery;
            _InvoicePaymentsMethodsCommand = invoicePaymentsMethodsCommand;
            _InvoiceMasterCommand = invoiceMasterCommand;
            _roundNumbers = roundNumbers;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
            _InvPersonsQuery = invPersonsQuery;
            this.filesOfInvoices = filesOfInvoices;
            ReceiptsHistory = receiptsHistory;
            _GLJournalEntryQuery = gLJournalEntryQuery;
            _GLJournalEntryDetailsCommand = gLJournalEntryDetailsCommand;
        }
        public async Task<ResponseResult> Handle(EditMultiCollectionReceiptsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var rec = _GlRecieptsQuery.TableNoTracking.FirstOrDefault(c => c.Id == request.Id);
            if (rec.IsBlock)
                return new ResponseResult
                {
                    Result = Result.NotExist,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "لا يمكن التعديل علي سند محذوف",
                        MessageEn = "You can not edit deleted receipt.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            if (rec == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExist,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "العنصر المطلوب غير موجود.",
                        MessageEn = "The requested item does not exist.",
                        titleAr = "العنصر غير موجود: خطأ",
                        titleEn = "Not Found: Error"
                    }
                };
            }

            MultiCollectionReceiptsHelper Helper = new MultiCollectionReceiptsHelper(_mediator, HistoryInvoiceService, _iUserInformation, _InvoiceMasterQuery, _GlRecieptsQuery, _InvPersonsQuery, _GLSafeQuery, _GLBankQuery);
            var _AddRequest = new AddMultiCollectionReceiptsRequest
            {
                isSafe = request.isSafe,
                docDate = request.docDate,
                Authority = request.Authority,
                personId = rec.PersonId ?? 0,
                PaymentMethodId = request.PaymentMethodId,
                safeId = request.isSafe ? request.safeId : null,
                bankId = !request.isSafe ? request.bankId : null,
                paperNumber = request.paperNumber ?? "",
                ChequeNumber = request.ChequeNumber,
                ChequeBankName = request.ChequeBankName,
                ChequeDate = request.ChequeDate,
                note = request.note ?? "",
                invoices = request.invoices
            };
            //validation 
            var isValied = await Helper.ValidationMethod(_AddRequest, true, request.Id);
            if (isValied != null)
                return isValied;
            var oldPaymentMethod = rec.PaymentMethodId;

            rec.SafeID = request.isSafe ? request.safeId : null;
            rec.BankId = !request.isSafe ? request.bankId : null;
            rec.PaperNumber = request.paperNumber;
            rec.RecieptDate = request.docDate;
            rec.Notes = request.note;
            rec.Amount = request.invoices.Sum(c => c.amount);
            rec.PaymentMethodId = request.PaymentMethodId;
            rec.ChequeNumber = request.ChequeNumber;
            rec.ChequeBankName = request.ChequeBankName;
            rec.ChequeDate = request.ChequeDate;
            rec.IsAccredit = true;
            rec.Authority = request.Authority;
            rec.BenefitId = request.personId;
            var masterRecSaved = await _GlRecieptsCommand.UpdateAsyn(rec);
            if (masterRecSaved)
            {
                await Helper.DeleteInvoiceMultiCollectionRec
                (request.Id,
                request.isSafe,
                oldPaymentMethod,
                _InvoicePaymentsMethodsCommand,
                _InvoiceMasterQuery,
                _InvoiceMasterCommand,
                _roundNumbers,
                InvoiceMasterHistoryRepositoryCommand,
                _GlRecieptsCommand,
                _InvoicePaymentsMethodsQuery,
                _GlRecieptsQuery
                );
                int[] filesId = { 0 };
                if(!string.IsNullOrEmpty(request.fileId) )
                {
                    filesId = request.fileId.Split(',').Select(c => int.Parse(c)).ToArray();
                }
                await filesOfInvoices.saveFilesOfInvoices(request.AttachedFile, userInfo.CurrentbranchId, request.isSafe ? FilesDirectories.SafeMultiCollectionReceipts : FilesDirectories.BankMultiCollectionReceipts, rec.Id, true, filesId.FirstOrDefault() != 0 ? filesId.ToList() : null, true);

                var invoices = _InvoiceMasterQuery.TableNoTracking;

                var invoicePaysBuild = await Helper.invoicbuilder(_AddRequest, rec, invoices);



                var listOfInvoicesRecs = invoicePaysBuild.recs;
                var recAdded =  await _GlRecieptsCommand.AddAsync(listOfInvoicesRecs);

                //var recAdded = await _GlRecieptsCommand.SaveChanges() > 0 ? true : false;

                if (recAdded)
                {
                    UpdateinvoiceForCollectionReceiptRequest invoicesForUpdate = invoicePaysBuild.invoices;
                    _InvoiceMasterQuery.ClearTracking();
                    var invoiceUpdated = await _mediator.Send(invoicesForUpdate);
                    if (invoiceUpdated.Result == Result.Success)
                    {
                        #region Journal Entry
                        int bankOrSafeFAId = 0;
                        if (request.isSafe)
                        {
                            bankOrSafeFAId = _GLSafeQuery.TableNoTracking.FirstOrDefault(c => c.Id == rec.SafeID).FinancialAccountId ?? 0;
                        }
                        else
                        {
                            bankOrSafeFAId = _GLBankQuery.TableNoTracking.FirstOrDefault(c => c.Id == rec.BankId).FinancialAccountId ?? 0;

                        }
                        string note = "";
                        if (request.isSafe)
                        {
                            note = "سند مجمع خزائن" + "_" + rec.RecieptType + (!string.IsNullOrEmpty(rec.Notes) ? "_" + rec.Notes : "");
                        }
                        else
                        {
                            note = "سند مجمع بنوك" + "_" + rec.RecieptType + (!string.IsNullOrEmpty(rec.Notes) ? "_" + rec.Notes : "");
                        }
                        await Helper.JournalEntryIntegration(new MultiCollectionReceiptsHelper.JournalEntryIntegrationDTO
                        {
                            MastreRec = rec,
                            invoices = invoices,
                            mediator = _mediator,
                            note = note,
                            safeOrBankFAId = bankOrSafeFAId,
                            userinfo = userInfo,
                            recInvoices = invoicePaysBuild.invoices.invoicesList,
                            isUpdate = true,
                            GLJournalEntryQuery = _GLJournalEntryQuery,
                            GLJournalEntryDetailsCommand = _GLJournalEntryDetailsCommand

                        });
                        #endregion
                        var listOfHistory = await Helper.historyBuilder(invoices, _AddRequest, rec, userInfo.CurrentbranchId);
                        var historyAdded = await InvoiceMasterHistoryRepositoryCommand.AddAsync(listOfHistory);
                    }
                    else
                    {
                        _GlRecieptsCommand.Rollback();
                        _GlRecieptsCommand.DeleteAsync(c => c.Id == rec.Id);

                        return new ResponseResult
                        {
                            Result = Result.Failed,
                            Alart = new Alart
                            {
                                AlartType = AlartType.error,
                                type = AlartShow.popup,
                                MessageAr = "حدث خطأ اثناء الحفظ",
                                MessageEn = "Error while saving",
                                titleAr = "خطأ",
                                titleEn = "Error"
                            }
                        };
                    }
                }
            }
            else
            {
                _GlRecieptsCommand.Rollback();
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "حدث خطأ اثناء الحفظ",
                        MessageEn = "Error while saving",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            }
            ReceiptsHistory.AddReceiptsHistory(
            rec.BranchId, rec.BenefitId, HistoryActions.Update, rec.PaymentMethodId,
            rec.UserId, (request.isSafe ? rec.SafeID ?? 0 : rec.BankId ?? 0), rec.Code,
            rec.RecieptDate, rec.Id, rec.RecieptType, rec.RecieptTypeId
            , rec.Signal, rec.IsBlock, rec.IsAccredit, rec.Serialize,
            rec.Authority, rec.Amount, 0, userInfo);
            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = ErrorMessagesAr.SaveSuccessfully,
                    MessageEn = ErrorMessagesEn.SaveSuccessfully
                }
            };
        }


    }
}
