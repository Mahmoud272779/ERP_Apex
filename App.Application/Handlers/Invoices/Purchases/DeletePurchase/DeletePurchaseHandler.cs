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
    public class DeletePurchaseHandler : IRequestHandler<DeletePurchaseRequest, ResponseResult>
    {
        private readonly IDeletePurchaseService DeletePurchaseService;

        public DeletePurchaseHandler(IDeletePurchaseService deletePurchaseService)
        {
            DeletePurchaseService = deletePurchaseService;
        }

        public async Task<ResponseResult> Handle(DeletePurchaseRequest request, CancellationToken cancellationToken)
        {
            return await DeletePurchaseService.DeletePurchase(request,(int)DocumentType.DeletePurchase);
        }
    }
}
