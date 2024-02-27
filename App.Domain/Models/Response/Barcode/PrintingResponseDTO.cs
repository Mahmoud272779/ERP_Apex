using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Barcode
{
    public class PrintingResponseDTO
    {
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double price { get; set; }
        public double priceWithVat { get; set; }
        public string categoryAr { get; set; }
        public string categoryEn { get; set; }
        public string expairDate { get; set; }
        public string BarcodeURL { get; set; }
    }
}
