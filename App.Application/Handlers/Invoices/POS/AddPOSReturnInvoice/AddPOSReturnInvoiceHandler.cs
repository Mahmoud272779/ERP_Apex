using App.Application.Services.Process.StoreServices.Invoices.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.AddPOSReturnInvoice
{
    public class AddPOSReturnInvoiceHandler : IRequestHandler<AddPOSReturnInvoiceRequest, ResponseResult>
    {
        private readonly IAddPOSInvoice AddPOSInvoiceService;

        public AddPOSReturnInvoiceHandler(IAddPOSInvoice addPOSInvoiceService)
        {
            AddPOSInvoiceService = addPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(AddPOSReturnInvoiceRequest request, CancellationToken cancellationToken)
        {
            return await AddPOSInvoiceService.AddPOSReturnInvoice(request);
        }
    }
}
