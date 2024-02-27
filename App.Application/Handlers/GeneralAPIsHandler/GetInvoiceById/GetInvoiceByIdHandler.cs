using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById
{
    public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdRequest, ResponseResult>
    {
        private readonly IGetInvoiceByIdService _IGetInvoiceByIdService;

        public GetInvoiceByIdHandler(IGetInvoiceByIdService iGetInvoiceByIdService)
        {
            _IGetInvoiceByIdService = iGetInvoiceByIdService;
        }

        public async Task<ResponseResult> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
        {
            return await _IGetInvoiceByIdService.GetInvoiceById(request.InvoiceId,request.isCopy , request.ForIOS);
        }
    }
}
