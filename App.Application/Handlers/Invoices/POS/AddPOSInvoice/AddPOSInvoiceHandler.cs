using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.AddPOSInvoice
{
    public class AddPOSInvoiceHandler : IRequestHandler<AddPOSInvoiceRequest, ResponseResult>
    {
        private readonly IAddSalesService addSalesService;

        public AddPOSInvoiceHandler(IAddSalesService addSalesService)
        {
            this.addSalesService = addSalesService;
        }

        public async Task<ResponseResult> Handle(AddPOSInvoiceRequest request, CancellationToken cancellationToken)
        {
           return await addSalesService.AddInvoiceForPOS(request);
        }
    }
}
