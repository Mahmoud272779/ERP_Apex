using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices.sales.DeleteSales
{
    public class DeleteSalesHandler : IRequestHandler<DeleteSalesRequest, ResponseResult>
    {
        private readonly IDeleteSalesService deleteSalesInvoice;

        public DeleteSalesHandler(IDeleteSalesService deleteSalesInvoice)
        {
            this.deleteSalesInvoice = deleteSalesInvoice;
        }

        public async Task<ResponseResult> Handle(DeleteSalesRequest request, CancellationToken cancellationToken)
        {
            return await deleteSalesInvoice.DeleteSales(request);
        }
    }
}
