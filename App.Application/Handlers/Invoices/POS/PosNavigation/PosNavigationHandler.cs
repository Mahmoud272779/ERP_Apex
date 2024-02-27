using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Invoices.POS.PosNavigation
{
    public class PosNavigationHandler : IRequestHandler<PosNavigationRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public PosNavigationHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(PosNavigationRequest request, CancellationToken cancellationToken)
        {
            return await GetPOSInvoiceService.POSNavigationStep(request.invoiceTypeId, request.invoiceId, request.stepType, request.branchId);
        }
    }
}
