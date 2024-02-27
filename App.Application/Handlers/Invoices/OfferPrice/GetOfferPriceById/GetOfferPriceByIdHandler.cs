using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById
{
    public  class GetOfferPriceByIdHandler : IRequestHandler<GetOfferPriceByIdRequest, ResponseResult>
    {
        private readonly IGetTempInvoiceByIdService _IGetInvoiceByIdService;

        public GetOfferPriceByIdHandler(IGetTempInvoiceByIdService iGetInvoiceByIdService)
        {
            _IGetInvoiceByIdService = iGetInvoiceByIdService;
        }

        public async Task<ResponseResult> Handle(GetOfferPriceByIdRequest request, CancellationToken cancellationToken)
        {
            return await _IGetInvoiceByIdService.GetInvoiceById(request.InvoiceId, request.isCopy);
        }
    }
}
