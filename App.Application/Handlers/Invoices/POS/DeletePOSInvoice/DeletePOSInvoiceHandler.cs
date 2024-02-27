using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.DeletePOSInvoice
{
    public class DeletePOSInvoiceHandler : IRequestHandler<DeletePOSInvoiceRequest, ResponseResult>
    {
        private readonly IDeleteSalesService deleteSalesInvoice;

        public DeletePOSInvoiceHandler(IDeleteSalesService deleteSalesInvoice)
        {
            this.deleteSalesInvoice = deleteSalesInvoice;
        }

        public async Task<ResponseResult> Handle(DeletePOSInvoiceRequest request, CancellationToken cancellationToken)
        {
            return await deleteSalesInvoice.DeleteInvoiceForPOS(request);
        }
    }
}
