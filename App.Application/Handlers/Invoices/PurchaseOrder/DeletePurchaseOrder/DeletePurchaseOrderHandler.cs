using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers
{
    public class DeletePurchaseOrderHandler : IRequestHandler<DeletePurchaseOrderRequest, ResponseResult>
    {
        private readonly IDeleteTempInvoiceService deleteOfferPriceService;

        public DeletePurchaseOrderHandler(IDeleteTempInvoiceService deleteOfferPriceService)
        {
            this.deleteOfferPriceService = deleteOfferPriceService;
        }

        public async Task<ResponseResult> Handle(DeletePurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            return await deleteOfferPriceService.DeleteTempInvoice(request,(int)DocumentType.DeletePurchaseOrder, InvoicesCode.DeletePurchaseOrder,(int)DocumentType.PurchaseOrder);
        }
    }
}
