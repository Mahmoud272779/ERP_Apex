using App.Domain.Models.Request.Barcode;
using App.Domain.Models.Response.Barcode;
using MediatR;

namespace App.Application.Handlers.Barcode.GetBarcodeItemsFromItemCard
{
    public class getBarcodeItemsFromItemCardRequest : PrintItemsBarcodeRequest,IRequest<List<PrintingResponseDTO>>
    {
    }
}
