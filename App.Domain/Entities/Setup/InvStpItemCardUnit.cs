using App.Domain.Entities.Process;
using System.Collections.Generic;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemCardUnit
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double ConversionFactor { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice1 { get; set; }
        public double SalePrice2 { get; set; }
        public double SalePrice3 { get; set; }
        public double SalePrice4 { get; set; }
        public string Barcode { get; set; }
        public bool WillDelete { get; set; }
        public virtual InvStpItemCardMaster Item { get; set; }
        public virtual ICollection<InvStpItemColorSize> ItemColorsSizes { get; set; }
        public virtual InvStpUnits Unit { get; set; }
       // public virtual InvoiceDetails InvoicesDetails { get; set; }

    }
}
