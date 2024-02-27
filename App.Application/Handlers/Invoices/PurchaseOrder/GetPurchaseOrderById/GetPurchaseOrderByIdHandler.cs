using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
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
    public  class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdRequest, ResponseResult>
    {
        private readonly IGetTempInvoiceByIdService _IGetInvoiceByIdService;

        public GetPurchaseOrderByIdHandler(IGetTempInvoiceByIdService iGetInvoiceByIdService)
        {
            _IGetInvoiceByIdService = iGetInvoiceByIdService;
        }

        public async Task<ResponseResult> Handle(GetPurchaseOrderByIdRequest request, CancellationToken cancellationToken)
        {
            return await _IGetInvoiceByIdService.GetInvoiceById(request.InvoiceId, request.isCopy);
        }
    }
}
