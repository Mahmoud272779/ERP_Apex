using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvPurchasesAdditionalCosts
    {
        public int PurchasesAdditionalCostsId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int AdditionalType { get; set; }
        public DateTime UTime { get; set; }

        public virtual ICollection<InvPurchaseAdditionalCostsRelation> InvPurchaseAdditionalCostsRelations { get; set; }

    }
}
