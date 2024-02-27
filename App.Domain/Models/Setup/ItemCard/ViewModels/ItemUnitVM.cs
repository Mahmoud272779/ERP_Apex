using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.ViewModels
{
    public class ItemUnitVM
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int UnitId { get; set; }
        [Required]
        public double ConversionFactor { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice1 { get; set; }
        public double SalePrice2 { get; set; }
        public double SalePrice3 { get; set; }
        public double SalePrice4 { get; set; }
        public string Barcode { get; set; }
        public virtual ICollection<ItemColorSizeVM> ItemColorsSizes { get; set; } = new List<ItemColorSizeVM>();

    }
}
