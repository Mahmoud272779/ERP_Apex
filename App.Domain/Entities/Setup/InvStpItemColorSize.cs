

using App.Domain.Entities.Process;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemColorSize
    {
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice1 { get; set; }
        public double SalePrice2 { get; set; }
        public double SalePrice3 { get; set; }
        public double SalePrice4 { get; set; }
        public string Barcode { get; set; }
        public bool WillDelete { get; set; }
        public virtual InvStpItemCardUnit ItemUnit { get; set; }
        public virtual InvColors Color { get; set; }
        public virtual InvSizes Size { get; set; }
        public virtual InvStpUnits Unit { get; set; }
        public virtual InvStpItemCardMaster ItemMaster { get; set; }
    }
}
