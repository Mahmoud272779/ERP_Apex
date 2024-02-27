using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvPurchaseAdditionalCostsRelation
    {
        public int PurchaseAdditionalCostsId { get; set; }
        public int InvoiceId { get; set; }
        public int AddtionalCostId { get; set; }
        [Description("Paid amount of invoice addition")]
        public double Amount { get; set; }//القيمة المدفوعة 
        public double Total { get; set; }
        public string CodeOfflinePOS { get; set; }

        public virtual InvoiceMaster InvoiceMaster { get; set; }
        public virtual InvPurchasesAdditionalCosts InvoiceAdditionalCosts { get; set; }


    }
}
