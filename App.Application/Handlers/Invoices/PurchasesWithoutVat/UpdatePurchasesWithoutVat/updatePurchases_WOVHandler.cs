using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class updatePurchases_WOVHandler : IRequestHandler<updatePurchases_WOVRequest, ResponseResult>
    {
        private readonly IUpdatePurchaseService UpdatePurchaseService;

        public updatePurchases_WOVHandler(IUpdatePurchaseService updatePurchaseService)
        {
            UpdatePurchaseService = updatePurchaseService;
        }

        public async Task<ResponseResult> Handle(updatePurchases_WOVRequest request, CancellationToken cancellationToken)
        {
            return await UpdatePurchaseService.UpdatePurchase(request,(int)DocumentType.wov_purchase);
        }
    }
}
