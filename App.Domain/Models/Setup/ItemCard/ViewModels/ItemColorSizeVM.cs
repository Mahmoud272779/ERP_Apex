using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.ViewModels
{
    public class ItemColorSizeVM
    {
        public int? ItemId { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice1 { get; set; }
        public double SalePrice2 { get; set; }
        public double SalePrice3 { get; set; }
        public double SalePrice4 { get; set; }
        public string Barcode { get; set; }
    }
}
