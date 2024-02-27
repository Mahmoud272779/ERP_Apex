using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
  public  class CheckBarcodeRequest : IRequest<ResponseResult>
    {
#nullable enable
        public string? Barcode { get; set; }
        public string?  ItemCode { get; set; }
        public string? NationalBarcode { get; set; }
        public int? ItemId { get; set; }

        public int? UnitId { get; set; }
        public CheckBarcodeRequest(string?  barcode , string? itemCode , string? nationalBarcode, int? ItemId, int? UnitId)
        {
            this.Barcode = barcode;
            this.ItemCode = itemCode;
            this.NationalBarcode = nationalBarcode;
            this.ItemId = ItemId;
            this.UnitId = UnitId;
        }
    }
}
