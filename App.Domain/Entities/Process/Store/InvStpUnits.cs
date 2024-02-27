using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvStpUnits
    {

        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the unit 1 if active 2 if inactive
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public DateTime UTime { get; set; }


        public virtual ICollection<InvStpItemCardUnit> CardUnits { get; set; }
        public virtual ICollection<InvStpItemColorSize> ItemColorsSizes { get; set; }
        public virtual ICollection<InvoiceDetails> InvoicesDetails { get; set; }
        public virtual ICollection<POSInvSuspensionDetails> POSInvSuspensionDetails { get; set; }
        public virtual ICollection<InvStpItemCardParts> ItemParts { get; set; }
        public virtual ICollection<OfferPriceDetails> OfferPriceDetails { get; set; } = new List<OfferPriceDetails>();

    }
    public class GetAllUnitsDTO
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the unit 1 if active 2 if inactive
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
    }
}
