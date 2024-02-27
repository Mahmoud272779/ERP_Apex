using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Purchases.GetPurchaseById
{
    public class GetPurchaseByIdHandler : IRequestHandler<GetPurchaseByIdRequest, ResponseResult>
    {
        private readonly IGetInvoiceByIdService GetPurchaseServiceById;

        public GetPurchaseByIdHandler(IGetInvoiceByIdService getPurchaseServiceById)
        {
            GetPurchaseServiceById = getPurchaseServiceById;
        }

        public async Task<ResponseResult> Handle(GetPurchaseByIdRequest request, CancellationToken cancellationToken)
        {
            return await GetPurchaseServiceById.GetInvoiceById(request.InvoiceId, request.isCopy);
        }
    }
}
