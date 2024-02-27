using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class AddItemUnitRequest : IRequest<ResponseResult>
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double ConversionFactor { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice1 { get; set; }
        public decimal SalePrice2 { get; set; }
        public decimal SalePrice3 { get; set; }
        public decimal SalePrice4 { get; set; }
        public string Barcode { get; set; }
    }
}
