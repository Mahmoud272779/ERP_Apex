using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvStpStores 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the store 1 if active 2 if inactive
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public DateTime UTime { get; set; }
        public virtual ICollection<InvStpItemCardStores> CardStores { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMaster { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMasterTo { get; set; }
        public virtual ICollection<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }

        public virtual ICollection<InvStoreBranch> StoreBranches { get; set; }
        public virtual ICollection<OtherSettingsStores> OtherSettingsStores { get; set; }
        public virtual ICollection<InvSerialTransaction> Serials { get; set; }
        public virtual ICollection<OfferPriceMaster> OfferPriceMaster { get; set; }


    }
}
