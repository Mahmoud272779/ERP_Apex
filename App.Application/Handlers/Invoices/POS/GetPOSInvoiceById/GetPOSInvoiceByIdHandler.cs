using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;

namespace App.Application.Handlers.Invoices.POS.GetPOSInvoiceById
{
    public class GetPOSInvoiceByIdHandler : IRequestHandler<GetPOSInvoiceByIdRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public GetPOSInvoiceByIdHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(GetPOSInvoiceByIdRequest request, CancellationToken cancellationToken)
        {
            return await GetPOSInvoiceService.GetPOSInvoiceById(request.InvoiceId, request.InvoiceCode,request.ForIOS);
        }
    }
}
