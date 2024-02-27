using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Infrastructure;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.MultiCollectionReceipts.AddMultiCollectionReceipts
{
    public class AddMultiCollectionReceiptsHandler : IRequestHandler<AddMultiCollectionReceiptsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryCommand<GlReciepts> _GlRecieptsCommand;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;


        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;

        private readonly iUserInformation _iUserInformation;
        private readonly IReceiptsService _IReceiptsService;
        private readonly IMediator _mediator;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryRepositoryCommand;

        public AddMultiCollectionReceiptsHandler(IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, iUserInformation iUserInformation, IRepositoryQuery<InvPersons> invPersonsQuery, IReceiptsService iReceiptsService, IRepositoryCommand<GlReciepts> glRecieptsCommand, IMediator mediator, IHistoryInvoiceService historyInvoiceService, IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryRepositoryCommand, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery)
        {
            _GlRecieptsQuery = glRecieptsQuery;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _InvPersonsQuery = invPersonsQuery;
            _IReceiptsService = iReceiptsService;
            _GlRecieptsCommand = glRecieptsCommand;
            _mediator = mediator;
            HistoryInvoiceService = historyInvoiceService;
            InvoiceMasterHistoryRepositoryCommand = invoiceMasterHistoryRepositoryCommand;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
        }
        public async Task<ResponseResult> Handle(AddMultiCollectionReceiptsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            MultiCollectionReceiptsHelper helper = new MultiCollectionReceiptsHelper(_mediator, HistoryInvoiceService, _iUserInformation, _InvoiceMasterQuery, _GlRecieptsQuery, _InvPersonsQuery, _GLSafeQuery, _GLBankQuery);
            //validation 
            var isValied = await helper.ValidationMethod(request);
            if (isValied != null)
                return isValied;
            var invoices = _InvoiceMasterQuery.TableNoTracking;

            double amount = request.invoices.Sum(c => c.amount);
            var masterRec = await _IReceiptsService.AddReceipt(new RecieptsRequest
            {
                SafeID = request.safeId,
                BankId = request.bankId,
                PaperNumber = request.paperNumber ?? "",
                RecieptDate = request.docDate,
                Notes = request.note,
                Amount = amount,
                Authority = request.Authority,
                PaymentMethodId = request.PaymentMethodId,
                ChequeNumber = request.ChequeNumber,
                ChequeBankName = request.ChequeBankName,
                ChequeDate = request.ChequeDate,
                RecieptTypeId = request.isSafe ? (int)Enums.DocumentType.SafeMultiCollectionReceipts : (int)Enums.DocumentType.BankMultiCollectionReceipts,
                UserId = userInfo.employeeId,
                BenefitId = request.personId,
                IsAccredit = true,
                invoices = request.invoices,
                AttachedFile = request.AttachedFile
            });
            if (masterRec.Id != null)
            {
                var rec = await _GlRecieptsQuery.GetByIdAsync(masterRec.Id);

                var invoicePaysBuild = await helper.invoicbuilder(request, rec, invoices);



                var listOfInvoicesRecs = invoicePaysBuild.recs;
                _GlRecieptsCommand.StartTransaction();
                _GlRecieptsCommand.AddRange(listOfInvoicesRecs);
                var recAdded = await _GlRecieptsCommand.SaveChanges() > 0 ? true : false;

                if (recAdded)
                {
                    _GlRecieptsCommand.CommitTransaction();
                    UpdateinvoiceForCollectionReceiptRequest invoicesForUpdate = invoicePaysBuild.invoices;
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
                        await helper.JournalEntryIntegration(new MultiCollectionReceiptsHelper.JournalEntryIntegrationDTO
                        {
                            MastreRec = rec,
                            invoices = invoices,
                            mediator = _mediator,
                            note = note,
                            safeOrBankFAId = bankOrSafeFAId,
                            userinfo = userInfo,
                            recInvoices = invoicePaysBuild.invoices.invoicesList

                        });
                        #endregion

                        var listOfHistory = await helper.historyBuilder(invoices, request, rec, userInfo.CurrentbranchId);
                        var historyAdded = await InvoiceMasterHistoryRepositoryCommand.AddAsync(listOfHistory);

                    }
                    else
                    {
                        _GlRecieptsCommand.Rollback();
                        _GlRecieptsCommand.DeleteAsync(c => c.Id == masterRec.Id);

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

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = ErrorMessagesAr.SaveSuccessfully,
                    MessageEn = ErrorMessagesEn.SaveSuccessfully,
                }
            };


        }
    }

}
