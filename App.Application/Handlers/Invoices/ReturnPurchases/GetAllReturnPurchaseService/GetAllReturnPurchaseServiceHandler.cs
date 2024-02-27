using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnPurchases.GetAllReturnPurchaseService
{
    public class GetAllReturnPurchaseServiceHandler : IRequestHandler<GetAllReturnPurchaseServiceRequest, ResponseResult>
    {
        private readonly IGetAllPurchasesService getAllPurchasesService;

        public GetAllReturnPurchaseServiceHandler(IGetAllPurchasesService getAllPurchasesService)
        {
            this.getAllPurchasesService = getAllPurchasesService;
        }

        public async Task<ResponseResult> Handle(GetAllReturnPurchaseServiceRequest request, CancellationToken cancellationToken)
        {
            return await getAllPurchasesService.GetAllPurchase(request,(int)DocumentType.ReturnPurchase);
        }
    }
}
