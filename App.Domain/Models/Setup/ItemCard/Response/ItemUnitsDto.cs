using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Response
{
    public class ItemUnitsDto
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double ConversionFactor { get; set; }
        public double SalePrice1 { get; set; }
    }
}
