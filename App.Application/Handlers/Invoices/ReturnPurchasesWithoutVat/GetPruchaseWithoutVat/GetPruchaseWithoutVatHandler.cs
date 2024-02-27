using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers 
{
    public class GetPruchaseWithoutVatHandler : IRequestHandler<GetPruchaseWithoutVatRequest, ResponseResult>
    {
        private readonly IGetPurchaseByCodeForReturn GetPurchaseByCodeService;

        public GetPruchaseWithoutVatHandler(IGetPurchaseByCodeForReturn getPurchaseByCodeService)
        {
            GetPurchaseByCodeService = getPurchaseByCodeService;
        }

        public async Task<ResponseResult> Handle(GetPruchaseWithoutVatRequest request, CancellationToken cancellationToken)
        {
            return await GetPurchaseByCodeService.GetPruchase(request.InvoiceCode,(int)DocumentType.wov_purchase);
        }
    }
}
