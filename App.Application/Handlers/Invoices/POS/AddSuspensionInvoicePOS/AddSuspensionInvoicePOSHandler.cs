using App.Application.Services.Process.StoreServices.Invoices.POS;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices.POS.AddSuspensionInvoicePOS
{
    public class AddSuspensionInvoicePOSHandler : IRequestHandler<AddSuspensionInvoicePOSRequest, ResponseResult>
    {
        private readonly IPOSInvSuspensionService InvSuspensionService;

        public AddSuspensionInvoicePOSHandler(IPOSInvSuspensionService invSuspensionService)
        {
            InvSuspensionService = invSuspensionService;
        }

        public async Task<ResponseResult> Handle(AddSuspensionInvoicePOSRequest request, CancellationToken cancellationToken)
        {
            return await InvSuspensionService.AddSuspensionInvoice(request);
        }
    }
}
