using App.Domain.Models.Response.GeneralLedger;
using App.Infrastructure.settings;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.MultiCollectionReceipts.GetByIdMultiCollectionReceipts
{
    public class GetByIdMultiCollectionReceiptsHandler : IRequestHandler<GetByIdMultiCollectionReceiptsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<GLRecieptFiles> _GLRecieptFilesQuery;
        private readonly IRoundNumbers _roundNumbers;
        private readonly iAuthorizationService _iAuthorizationService;
        public GetByIdMultiCollectionReceiptsHandler(IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<GLRecieptFiles> gLRecieptFilesQuery, IRoundNumbers roundNumbers, iAuthorizationService iAuthorizationService)
        {
            _GlRecieptsQuery = glRecieptsQuery;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
            _InvPersonsQuery = invPersonsQuery;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _GLRecieptFilesQuery = gLRecieptFilesQuery;
            _roundNumbers = roundNumbers;
            _iAuthorizationService = iAuthorizationService;
        }
        public async Task<ResponseResult> Handle(GetByIdMultiCollectionReceiptsRequest request, CancellationToken cancellationToken)
        {
            var data = _GlRecieptsQuery.TableNoTracking
                .Include(c => c.person)
                .Include(c => c.PaymentMethods)
                .Where(c => c.RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || c.RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts)
                .Where(c => c.Id == request.Id || c.MultiCollectionReceiptParentId == request.Id);
            var isAuth = await _iAuthorizationService.isAuthorized(0, data.FirstOrDefault(c => c.MultiCollectionReceiptParentId == null).RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts ? (int)SubFormsIds.SafeMultiCollectionReceipt : (int)SubFormsIds.BankMultiCollectionReceipt, Opretion.Open);
            if (isAuth != null)
                return isAuth;
            if (!data.Any())
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
            var MasterRec = data.FirstOrDefault(c => c.MultiCollectionReceiptParentId == null);
            var safes = _GLSafeQuery.TableNoTracking.FirstOrDefault(c => c.Id == (MasterRec.SafeID ?? 0));
            var banks = _GLBankQuery.TableNoTracking.FirstOrDefault(c => c.Id == (MasterRec.BankId ?? 0));
            var persons = _InvPersonsQuery.TableNoTracking.FirstOrDefault(c => c.Id == MasterRec.BenefitId);
            var invoices = _InvoiceMasterQuery.TableNoTracking;

            List<invoice> RecInvoices = new List<invoice>();
            foreach (var item in data.Where(c => c.MultiCollectionReceiptParentId == MasterRec.Id))
            {
                var currentInvoice = invoices.FirstOrDefault(c => c.InvoiceId == item.ParentId);
                RecInvoices.Add(new invoice
                {
                    Id = currentInvoice.InvoiceId,
                    invoiceType = currentInvoice.InvoiceType,
                    net = _roundNumbers.GetRoundNumber(currentInvoice.Net),
                    invoicePaid = _roundNumbers.GetRoundNumber(currentInvoice.Paid - item.Amount),
                    recPaid = _roundNumbers.GetRoundNumber(item.Amount)
                });
            }
            var recFiles = _GLRecieptFilesQuery.TableNoTracking.Where(c => c.RecieptId == MasterRec.Id).Select(c => new recieptsFiles
            {
                fileId = c.Id,
                fileExtensions = c.FileExtensions,
                fileLink = c.FileLink,
                fileName = c.FileName
            })
            .ToList();
            GetByIdMultiCollectionReceiptsResponseDTO multiCollectionRec = new GetByIdMultiCollectionReceiptsResponseDTO
            {
                Id = MasterRec.Id,

                RecieptType = MasterRec.RecieptType,
                isSafe = MasterRec.SafeID != null ? true : false,
                safeOrBankObj = MasterRec.SafeID != null ? new safeOrBankObj
                {
                    Id = MasterRec.SafeID ?? 0,
                    arabicName = safes.ArabicName,
                    latinName = safes.LatinName,
                } :
                new safeOrBankObj
                {
                    Id = MasterRec.BankId ?? 0,
                    arabicName = banks.ArabicName,
                    latinName = banks.LatinName,
                },
                RecieptDate = MasterRec.RecieptDate.ToString(defultData.datetimeFormat),
                PaperNumber = MasterRec.PaperNumber,
                Authority = MasterRec.Authority,
                note = MasterRec.Notes,
                Benefit = MasterRec.BenefitId != 0 ? new Benefit
                {
                    Id = persons.Id,
                    arabicName = persons.ArabicName,
                    latinName = persons.LatinName,
                } :
                new Benefit
                {
                    Id = 0,
                    arabicName = "الكل",
                    latinName = "All",
                },
                amount = _roundNumbers.GetDefultRoundNumber(MasterRec.Amount),
                paymentMethod = new paymentMethod
                {
                    Id = MasterRec.PaymentMethodId,
                    arabicName = MasterRec.PaymentMethods.ArabicName,
                    latinName = MasterRec.PaymentMethods.LatinName
                },
                invoices = RecInvoices,
                recieptsFiles = recFiles,
                branchId = MasterRec.BranchId,
                code = MasterRec.Code,

                signal = MasterRec.Signal,
                isBlock = MasterRec.IsBlock,
                isAccredit = MasterRec.IsAccredit,
                serialize = MasterRec.Serialize,
                subTypeId = MasterRec.SubTypeId,
                documentDate = MasterRec.RecieptDate.ToString("yyyy/MM/dd")
            };
            return new ResponseResult
            {
                Data = multiCollectionRec,
                Result = Result.Success
            };
        }
    }
}
