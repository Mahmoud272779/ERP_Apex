using App.Application.Handlers.Invoices.sales;
using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices
{
    internal class GetSalesByIdHandler : IRequestHandler<GetSalesByIdRequest, ResponseResult>
    {
        private readonly IGetInvoiceByIdService GetSalesServiceById;

        public GetSalesByIdHandler(IGetInvoiceByIdService getSalesServiceById)
        {
            GetSalesServiceById = getSalesServiceById;
        }

        public async Task<ResponseResult> Handle(GetSalesByIdRequest request, CancellationToken cancellationToken)
        {
            return await GetSalesServiceById.GetInvoiceById(request.InvoiceId, request.isCopy);
        }
    }
}
