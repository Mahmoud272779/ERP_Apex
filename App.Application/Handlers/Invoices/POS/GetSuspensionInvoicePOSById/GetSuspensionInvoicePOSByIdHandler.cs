using App.Application.Services.Process.StoreServices.Invoices.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOSById
{
    public class GetSuspensionInvoicePOSByIdHandler : IRequestHandler<GetSuspensionInvoicePOSByIdRequest, ResponseResult>
    {
        private readonly IPOSInvSuspensionService InvSuspensionService;

        public GetSuspensionInvoicePOSByIdHandler(IPOSInvSuspensionService invSuspensionService)
        {
            InvSuspensionService = invSuspensionService;
        }

        public async Task<ResponseResult> Handle(GetSuspensionInvoicePOSByIdRequest request, CancellationToken cancellationToken)
        {
            return await InvSuspensionService.GetSuspensionInvoiceById(request.Id);
        }
    }
}
