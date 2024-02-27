using App.Domain.Models.Request.Barcode;
using App.Domain.Models.Response.Barcode;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Barcode.GetBarcodeItemsFromInvoices
{
    public class GetBarcodeItemsFromInvoicesRequest : PrintItemsBarcodeRequest, IRequest<List<PrintingResponseDTO>>
    {
    }
}
