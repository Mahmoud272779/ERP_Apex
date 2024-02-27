using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class GetPersonBalanceHandler : IRequestHandler<GetPersonBalanceRequest, double>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IReceiptsService receiptsService;

        public GetPersonBalanceHandler(iUserInformation iUserInformation, IReceiptsService receiptsService)
        {
            _iUserInformation = iUserInformation;
            this.receiptsService = receiptsService;
        }
        public async Task<double> Handle(GetPersonBalanceRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            double balance = 0;
            if (Lists.purchasesWithOutDeleteInvoicesList.Contains( request.invoiceTypeId )&& userInfo.otherSettings.purchaseShowBalanceOfPerson)
            {
                var res = await receiptsService.GetReceiptBalanceForBenifit((int)AuthorityTypes.suppliers,request.personId);
                balance = res.Total;
            }
            return balance;
        }
    }
}
