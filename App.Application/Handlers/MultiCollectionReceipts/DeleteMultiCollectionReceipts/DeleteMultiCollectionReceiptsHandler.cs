using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.MultiCollectionReceipts.DeleteMultiCollectionReceipts
{
    public class DeleteMultiCollectionReceiptsHandler : IRequestHandler<DeleteMultiCollectionReceiptsRequest, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _GLJournalEntryQuery;

        private readonly IRepositoryCommand<InvoicePaymentsMethods> _InvoicePaymentsMethodsCommand;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryCommand<InvoiceMaster> _InvoiceMasterCommand;
        private readonly IRoundNumbers _roundNumbers;
        private readonly IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryRepositoryCommand;
        private readonly IRepositoryCommand<GlReciepts> _GlRecieptsCommand;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsQuery;
        private readonly iUserInformation _iuserInforrmation;
        private readonly IHistoryReceiptsService ReceiptsHistory;


        public DeleteMultiCollectionReceiptsHandler(IMediator mediator, IHistoryInvoiceService historyInvoiceService, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryCommand<InvoicePaymentsMethods> invoicePaymentsMethodsCommand, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryCommand<InvoiceMaster> invoiceMasterCommand, IRoundNumbers roundNumbers, IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryRepositoryCommand, IRepositoryCommand<GlReciepts> glRecieptsCommand, IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentsMethodsQuery, iUserInformation iuserInforrmation, iAuthorizationService iAuthorizationService, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IHistoryReceiptsService receiptsHistory, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery)
        {
            _mediator = mediator;
            HistoryInvoiceService = historyInvoiceService;
            _GlRecieptsQuery = glRecieptsQuery;
            _InvoicePaymentsMethodsCommand = invoicePaymentsMethodsCommand;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _InvoiceMasterCommand = invoiceMasterCommand;
            _roundNumbers = roundNumbers;
            InvoiceMasterHistoryRepositoryCommand = invoiceMasterHistoryRepositoryCommand;
            _GlRecieptsCommand = glRecieptsCommand;
            _InvoicePaymentsMethodsQuery = invoicePaymentsMethodsQuery;
            _iuserInforrmation = iuserInforrmation;
            _iAuthorizationService = iAuthorizationService;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
            _InvPersonsQuery = invPersonsQuery;
            ReceiptsHistory = receiptsHistory;
            _GLJournalEntryQuery = gLJournalEntryQuery;
        }
        public async Task<ResponseResult> Handle(DeleteMultiCollectionReceiptsRequest request, CancellationToken cancellationToken)
        {
            MultiCollectionReceiptsHelper Helper = new MultiCollectionReceiptsHelper(_mediator, HistoryInvoiceService,_iuserInforrmation,_InvoiceMasterQuery, _GlRecieptsQuery,_InvPersonsQuery, _GLSafeQuery, _GLBankQuery);
            var rec = _GlRecieptsQuery.TableNoTracking.FirstOrDefault(c => c.Id == request.Id);

            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, rec.SafeID != null ? (int)SubFormsIds.SafeMultiCollectionReceipt : (int)SubFormsIds.BankMultiCollectionReceipt, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            var userInfo = await _iuserInforrmation.GetUserInformation();

            if (rec == null)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "العنصر المطلوب غير موجود.",
                        MessageEn = "The requested item does not exist.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            }
            var deletedOldRecords = await Helper.DeleteInvoiceMultiCollectionRec
                           (request.Id,
                           rec.SafeID != null ? true : false,
                           rec.PaymentMethodId, 
                           _InvoicePaymentsMethodsCommand,
                           _InvoiceMasterQuery,
                           _InvoiceMasterCommand,
                           _roundNumbers,
                           InvoiceMasterHistoryRepositoryCommand,
                           _GlRecieptsCommand,
                           _InvoicePaymentsMethodsQuery,
                           _GlRecieptsQuery
                           );
            if (deletedOldRecords)
            {
                rec.IsBlock = true;
                var recUpdated = await _GlRecieptsCommand.UpdateAsyn(rec);
                if (!recUpdated)
                {
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = ErrorMessagesAr.ErrorSaving,
                            MessageEn = ErrorMessagesEn.ErrorSaving,
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                }
            }


            #region Block JouranlEntry 
            var journalEntry = _GLJournalEntryQuery.TableNoTracking.Where(c => c.ReceiptsId == rec.Id);
            await _mediator.Send(new BlockJournalEntryReqeust
            {
                Ids = journalEntry.Select(c=> c.Id).ToArray()
            });
            #endregion

            ReceiptsHistory.AddReceiptsHistory(
                                         rec.BranchId, rec.BenefitId, HistoryActions.Delete, rec.PaymentMethodId,
                                         rec.UserId, rec.BankId != null ? rec.BankId.Value : rec.SafeID.Value,
                                         rec.Code, rec.RecieptDate, rec.Id, rec.RecieptType, rec.RecieptTypeId,
                                         rec.Signal, rec.IsBlock, rec.IsAccredit, rec.Serialize,
                                         rec.Authority, rec.Amount, rec.SubTypeId, userInfo);
            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = ErrorMessagesAr.DeletedSuccessfully,
                    MessageEn = ErrorMessagesEn.DeletedSuccessfully
                }
            };
        }
    }
}
