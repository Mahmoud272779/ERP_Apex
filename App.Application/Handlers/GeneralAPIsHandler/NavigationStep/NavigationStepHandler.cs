using App.Application.Helpers.POSHelper;
using App.Application.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.NavigationStep
{
    public class NavigationStepHandler : IRequestHandler<NavigationStepRequest, ResponseResult>
    {
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;

        public NavigationStepHandler(iUserInformation userinformation, IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery)
        {
            Userinformation = userinformation;
            InvoiceMasterRepositoryQuery = invoiceMasterRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(NavigationStepRequest request, CancellationToken cancellationToken)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            bool showOthoerInvoice = POSHelper.showOthorInv(request.invoiceTypeId, userInfo);
            int invoiceTypeDel = (request.invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : request.invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : request.invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            int LastCode = InvoiceMasterRepositoryQuery.GetMaxCode(a => a.InvoiceId, q =>
                q.BranchId == userInfo.CurrentbranchId
                && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && (request.NextCode ? q.InvoiceId > request.invoiceCode : q.InvoiceId < request.invoiceCode)
                && ((q.InvoiceTypeId == request.invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));
            return new ResponseResult() { Data = LastCode };
        }
    }
}
