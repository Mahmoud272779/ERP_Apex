using App.Application.Services.Process.StoreServices.Invoices.POS;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOS
{
    public class GetSuspensionInvoicePOSHandler : IRequestHandler<GetSuspensionInvoicePOSRequest, ResponseResult>
    {
        private readonly IPOSInvSuspensionService InvSuspensionService;

        public GetSuspensionInvoicePOSHandler(IPOSInvSuspensionService invSuspensionService)
        {
            InvSuspensionService = invSuspensionService;
        }

        public async Task<ResponseResult> Handle(GetSuspensionInvoicePOSRequest request, CancellationToken cancellationToken)
        {
            return await InvSuspensionService.GetSuspensionInvoice(request.PageNumber, request.PageSize);
        }
    }
}
