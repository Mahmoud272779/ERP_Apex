using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnPurchases.GetPruchase
{
    public class GetPruchaseHandler : IRequestHandler<GetPruchaseRequest, ResponseResult>
    {
        private readonly IGetPurchaseByCodeForReturn GetPurchaseByCodeService;

        public GetPruchaseHandler(IGetPurchaseByCodeForReturn getPurchaseByCodeService)
        {
            GetPurchaseByCodeService = getPurchaseByCodeService;
        }

        public async Task<ResponseResult> Handle(GetPruchaseRequest request, CancellationToken cancellationToken)
        {
            return await GetPurchaseByCodeService.GetPruchase(request.InvoiceCode,(int)DocumentType.Purchase);
        }
    }
}
