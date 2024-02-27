using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class GetAllPurchaseOrderHandler : IRequestHandler<GetAllPurchaseOrderRequest, ResponseResult>
    {
        private readonly IGetAllTempInvoicesServices getAllPurchaseOrderServices;

        public GetAllPurchaseOrderHandler(IGetAllTempInvoicesServices getAllPurchaseOrderServices)
        {
            this.getAllPurchaseOrderServices = getAllPurchaseOrderServices;
        }

        public async Task<ResponseResult> Handle(GetAllPurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            return await getAllPurchaseOrderServices.GetAllTempInvoices(request ,(int)DocumentType.PurchaseOrder, (int)DocumentType.DeletePurchaseOrder);
        }
    }
}
