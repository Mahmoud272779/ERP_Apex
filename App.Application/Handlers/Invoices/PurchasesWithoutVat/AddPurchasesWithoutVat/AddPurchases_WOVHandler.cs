using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.AddPurchases
{
    public class AddPurchases_WOVHandler : IRequestHandler<AddPurchases_WOVRequest, ResponseResult>
    {
        private readonly IAddPurchaseService PurchaseService;

        public AddPurchases_WOVHandler(IAddPurchaseService purchaseService)
        {
            PurchaseService = purchaseService;
        }

        public async Task<ResponseResult> Handle(AddPurchases_WOVRequest request, CancellationToken cancellationToken)
        {
            return await PurchaseService.AddPurchase(request,(int)DocumentType.wov_purchase);
        }
    }
}
