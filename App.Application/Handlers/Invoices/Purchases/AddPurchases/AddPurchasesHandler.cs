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
    public class AddPurchasesHandler : IRequestHandler<AddPurchasesRequest, ResponseResult>
    {
        private readonly IAddPurchaseService PurchaseService;

        public AddPurchasesHandler(IAddPurchaseService purchaseService)
        {
            PurchaseService = purchaseService;
        }

        public async Task<ResponseResult> Handle(AddPurchasesRequest request, CancellationToken cancellationToken)
        {
            int invoiceTypeId = (int)DocumentType.Purchase;
            if (request.IsPurchasesWithoutVat)
                invoiceTypeId = (int)DocumentType.wov_purchase;
            return await PurchaseService.AddPurchase(request, invoiceTypeId);
        }
    }
}
