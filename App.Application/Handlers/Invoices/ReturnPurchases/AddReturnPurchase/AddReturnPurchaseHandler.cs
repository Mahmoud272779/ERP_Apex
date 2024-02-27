using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnPurchases.AddReturnPurchase
{
    public class AddReturnPurchaseHandler : IRequestHandler<AddReturnPurchaseRequest, ResponseResult>
    {
        private readonly IAddReturnPurchaseService AddReturnPurchaseService;

        public AddReturnPurchaseHandler(IAddReturnPurchaseService addReturnPurchaseService)
        {
            AddReturnPurchaseService = addReturnPurchaseService;
        }

        public async Task<ResponseResult> Handle(AddReturnPurchaseRequest request, CancellationToken cancellationToken)
        {
            int invoiceTypeId = (int)DocumentType.ReturnPurchase;
            if (request.IsPurchasesWithoutVat)
                invoiceTypeId = (int)DocumentType.ReturnWov_purchase;
            return await AddReturnPurchaseService.AddReturnPurchase(request, invoiceTypeId);
        }
    }
}
