using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Setup;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace App.Domain.Entities.Process
{
    public class InvSizes
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the size 1 if active 2 if inactive
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public virtual ICollection<InvoiceDetails> InvoicesDetails { get; set; }
        public virtual ICollection<POSInvSuspensionDetails> POSInvSuspensionDetails { get; set; }
        public virtual ICollection<InvStpItemColorSize> Items { get; set; }
        public virtual ICollection<OfferPriceDetails> OfferPriceDetails { get; set; } = new List<OfferPriceDetails>();

    }
}
