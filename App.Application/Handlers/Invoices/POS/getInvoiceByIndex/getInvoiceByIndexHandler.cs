using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.getInvoiceByIndex
{
    public class getInvoiceByIndexHandler : IRequestHandler<getInvoiceByIndexRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public getInvoiceByIndexHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(getInvoiceByIndexRequest request, CancellationToken cancellationToken)
        {
            return await GetPOSInvoiceService.POSNavigationStepIndex(request.invoiceTypeId, request.index, request.branchId);
        }
    }
}
