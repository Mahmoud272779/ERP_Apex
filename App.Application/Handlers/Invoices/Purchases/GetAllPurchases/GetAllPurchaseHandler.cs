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
    public class GetAllPurchaseHandler : IRequestHandler<GetAllPurchaseRequest, ResponseResult>
    {
        private readonly IGetAllPurchasesService GetAllPurchaseService;

        public GetAllPurchaseHandler(IGetAllPurchasesService getAllPurchaseService)
        {
            GetAllPurchaseService = getAllPurchaseService;
        }

        public async Task<ResponseResult> Handle(GetAllPurchaseRequest request, CancellationToken cancellationToken)
        {
            return await GetAllPurchaseService.GetAllPurchase(request,(int)DocumentType.Purchase);
        }
    }
}
