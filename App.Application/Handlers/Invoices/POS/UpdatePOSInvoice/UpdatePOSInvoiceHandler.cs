using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.UpdatePOSInvoice
{
    public class UpdatePOSInvoiceHandler : IRequestHandler<UpdatePOSInvoiceRequest, ResponseResult>
    {
        private readonly IUpdateSalesService updateSalesService;

        public UpdatePOSInvoiceHandler(IUpdateSalesService updateSalesService)
        {
            this.updateSalesService = updateSalesService;
        }

        public async Task<ResponseResult> Handle(UpdatePOSInvoiceRequest request, CancellationToken cancellationToken)
        {
            return await updateSalesService.UpdateInvoiceForPOS(request);
        }
    }
}
