using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices
{
    public class AddReturnPurchaseWithoutVatHandler : IRequestHandler<AddReturnPurchaseWithoutVatRequest, ResponseResult>
    {
        private readonly IAddReturnPurchaseService AddReturnPurchaseService;

        public AddReturnPurchaseWithoutVatHandler(IAddReturnPurchaseService addReturnPurchaseService)
        {
            AddReturnPurchaseService = addReturnPurchaseService;
        }

        public async Task<ResponseResult> Handle(AddReturnPurchaseWithoutVatRequest request, CancellationToken cancellationToken)
        {
            return await AddReturnPurchaseService.AddReturnPurchase(request,(int)DocumentType.ReturnWov_purchase);
        }
    }
}
