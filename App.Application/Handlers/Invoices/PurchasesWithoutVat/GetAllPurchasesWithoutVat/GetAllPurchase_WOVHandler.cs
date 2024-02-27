using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetAllPurchases
{
    public class GetAllPurchase_WOVHandler : IRequestHandler<GetAllPurchase_WOVRequest, ResponseResult>
    {
        private readonly IGetAllPurchasesService GetAllPurchaseService;

        public GetAllPurchase_WOVHandler(IGetAllPurchasesService getAllPurchaseService)
        {
            GetAllPurchaseService = getAllPurchaseService;
        }

        public async Task<ResponseResult> Handle(GetAllPurchase_WOVRequest request, CancellationToken cancellationToken)
        {
            return await GetAllPurchaseService.GetAllPurchase(request,(int)DocumentType.wov_purchase);
        }
    }
}
