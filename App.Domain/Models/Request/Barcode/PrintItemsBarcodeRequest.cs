using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Barcode
{
    public class PrintItemsBarcodeRequest
    {
        public int DesignId { get; set; }
        public bool isInvoice { get; set; }
        public List<PrintItemsBarcodeRequestDetalies> PrintItemsBarcodeRequestDetalies { get; set; }
    }
    public class PrintItemsBarcodeRequestDetalies
    {
        public int itemId { get; set; }
        public int count { get; set; }
        public DateTime? expairDate { get; set; } = DateTime.Now;
        public int? ivoiceDetaliesId { get; set; }
        public int? unitId { get; set; }    
    }
}
