using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.DeletePurchase
{
    public class DeletePurchase_WOVHandler : IRequestHandler<DeletePurchase_WOVRequest, ResponseResult>
    {
        private readonly IDeletePurchaseService DeletePurchaseService;

        public DeletePurchase_WOVHandler(IDeletePurchaseService deletePurchaseService)
        {
            DeletePurchaseService = deletePurchaseService;
        }

        public async Task<ResponseResult> Handle(DeletePurchase_WOVRequest request, CancellationToken cancellationToken)
        {
            return await DeletePurchaseService.DeletePurchase(request,(int)DocumentType.DeleteWov_purchase);
        }
    }
}
