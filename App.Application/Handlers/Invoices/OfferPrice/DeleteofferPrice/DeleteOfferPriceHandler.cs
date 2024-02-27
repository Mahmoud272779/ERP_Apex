using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers
{
    public class DeleteOfferPriceHandler : IRequestHandler<DeleteOfferPriceRequest, ResponseResult>
    {
        private readonly IDeleteTempInvoiceService deleteOfferPriceService;

        public DeleteOfferPriceHandler(IDeleteTempInvoiceService deleteOfferPriceService)
        {
            this.deleteOfferPriceService = deleteOfferPriceService;
        }

        public async Task<ResponseResult> Handle(DeleteOfferPriceRequest request, CancellationToken cancellationToken)
        {
            return await deleteOfferPriceService.DeleteTempInvoice(request,(int)DocumentType.DeleteOfferPrice, InvoicesCode.DeleteOfferPrice,(int)DocumentType.OfferPrice);
        }
    }
}
