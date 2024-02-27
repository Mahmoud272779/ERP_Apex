using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetAllPOSReturnInvoices
{
    public class GetAllPOSReturnInvoicesHandler : IRequestHandler<GetAllPOSReturnInvoicesRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public GetAllPOSReturnInvoicesHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(GetAllPOSReturnInvoicesRequest request, CancellationToken cancellationToken)
        {
            return await GetPOSInvoiceService.GetAllPOSInvoices(request);
        }
    }
}
