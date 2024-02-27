using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemCardMaster 
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string NationalBarcode { get; set; }
        public int TypeId { get; set; }
        public bool UsedInSales { get; set; }
        public int? DepositeUnit { get; set; }
        public int? WithdrawUnit { get; set; }
        public int? ReportUnit { get; set; }
        public double VAT { get; set; }
        public bool ApplyVAT { get; set; }
        public string Model { get; set; }
        public int? DefaultStoreId { get; set; }
        public string Description { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int GroupId { get; set; }
        public int Status { get; set; }//Represent the status of the ItemCard 1 if active 2 if inactive
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public int BranchId { get; set; }
        public bool CanDelete { get; set; }
        public DateTime UTime { get; set; }

        public virtual InvStorePlaces StorePlace { get; set; }
        public virtual ICollection<InvStpItemCardUnit> Units { get; set; }
        public virtual ICollection<InvStpItemCardStores> Stores { get; set; }
        public virtual InvCategories Category { get; set; }
        public virtual ICollection<InvoiceDetails> InvoicesDetails { get; set; } = new List<InvoiceDetails>();
        public virtual ICollection<POSInvSuspensionDetails> POSInvSuspensionDetails { get; set; } = new List<POSInvSuspensionDetails>();
        public virtual ICollection<InvSerialTransaction> Serials { get; set; } = new List<InvSerialTransaction>();
        public virtual ICollection<InvStpItemCardParts> Parts { get; set; } = new List<InvStpItemCardParts>();
        public virtual ICollection<InvStpItemCardParts> ItemParts { get; set; } = new List<InvStpItemCardParts>();
        public virtual ICollection<InvStpItemColorSize> ColorsSizes { get; set; } = new List<InvStpItemColorSize>();
        public virtual ICollection<OfferPriceDetails> OfferPriceDetails { get; set; } = new List<OfferPriceDetails>();

        //  public virtual ICollection<InvStpItemCardSerials> ItemSerials { get; set; }
    }
}
