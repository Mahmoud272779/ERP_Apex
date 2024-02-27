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
    public class GetAllReturnPurchaseWithoutVatHandler : IRequestHandler<GetAllReturnPurchaseWithoutVatRequest, ResponseResult>
    {
        private readonly IGetAllPurchasesService getAllPurchasesService;

        public GetAllReturnPurchaseWithoutVatHandler(IGetAllPurchasesService getAllPurchasesService)
        {
            this.getAllPurchasesService = getAllPurchasesService;
        }

        public async Task<ResponseResult> Handle(GetAllReturnPurchaseWithoutVatRequest request, CancellationToken cancellationToken)
        {
            return await getAllPurchasesService.GetAllPurchase(request,(int)DocumentType.ReturnWov_purchase);
        }
    }
}
