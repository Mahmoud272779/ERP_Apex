using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;

namespace App.Application.Handlers.Invoices.POS.GetReturnPOS
{
    public class GetReturnPOSHandler : IRequestHandler<GetReturnPOSRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public GetReturnPOSHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(GetReturnPOSRequest request, CancellationToken cancellationToken)
        {
            return await GetPOSInvoiceService.GetPOSReturnInvoice(request.InvoiceCode,request.ForIOS);
        }
    }
}
