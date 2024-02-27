using App.Application.Services.Process.StoreServices.Invoices.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.AddPOSTotalReturnInvoice
{
    public class AddPOSTotalReturnInvoiceHandler : IRequestHandler<AddPOSTotalReturnInvoiceRequest, ResponseResult>
    {
        private readonly IAddPOSInvoice AddPOSInvoiceService;

        public AddPOSTotalReturnInvoiceHandler(IAddPOSInvoice addPOSInvoiceService)
        {
            AddPOSInvoiceService = addPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(AddPOSTotalReturnInvoiceRequest request, CancellationToken cancellationToken)
        {
            return await AddPOSInvoiceService.AddPOSTotalReturnInvoice(request.Id);
        }
    }
}
