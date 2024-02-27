using MediatR;
using System.Threading;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.InvoicesHelper.CheckInvoiceExistance
{
    public class CheckInvoiceExistanceHandler : IRequestHandler<CheckInvoiceExistanceRequest, ResponseResult>
    {
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;

        public CheckInvoiceExistanceHandler(iUserInformation userinformation, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery)
        {
            Userinformation = userinformation;
            this.invoiceMasterQuery = invoiceMasterQuery;
        }

        public async Task<ResponseResult> Handle(CheckInvoiceExistanceRequest request, CancellationToken cancellationToken)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            try
            {
                var invoiceExist = await invoiceMasterQuery.TableNoTracking.FirstOrDefaultAsync(q => ((q.InvoiceType == request.invoiceType || q.Code.ToString() == request.invoiceType
                || (request.InvoiceTypeId == (int)DocumentType.POS ? q.CodeOfflinePOS == request.invoiceType : false)) && q.InvoiceTypeId == request.InvoiceTypeId && q.BranchId == userInfo.CurrentbranchId)
                && Lists.MainInvoiceForReturn.Contains(q.InvoiceTypeId));

                return new ResponseResult
                {
                    Result = invoiceExist == null ? Result.NoDataFound :
                         (invoiceExist.InvoiceSubTypesId == (int)SubType.TotalReturn ? Result.InvoiceTotalReturned : (invoiceExist.IsDeleted ? Result.Deleted : Result.Success))
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Note = ex.Message,
                    Result = Result.Failed
                };
            }

        }
    }
}
