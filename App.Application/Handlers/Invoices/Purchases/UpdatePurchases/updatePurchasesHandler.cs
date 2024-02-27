using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.UpdatePurchases
{
    public class updatePurchasesHandler : IRequestHandler<updatePurchasesRequest, ResponseResult>
    {
        private readonly IUpdatePurchaseService UpdatePurchaseService;

        public updatePurchasesHandler(IUpdatePurchaseService updatePurchaseService)
        {
            UpdatePurchaseService = updatePurchaseService;
        }

        public async Task<ResponseResult> Handle(updatePurchasesRequest request, CancellationToken cancellationToken)
        {
            return await UpdatePurchaseService.UpdatePurchase(request,(int)DocumentType.Purchase);
        }
    }
}
