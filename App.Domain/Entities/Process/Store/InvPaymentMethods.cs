using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvPaymentMethods 
    {

        public int PaymentMethodId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int? SafeId { get; set; }
        public int? BankId { get; set; }
        public int Status { get; set; }
        public bool CanDelete { get; set; }=false;
        public DateTime UTime { get; set; }
        public virtual GLSafe safe { get; set; }
        public virtual GLBank bank { get; set; }
        public virtual ICollection<InvoicePaymentsMethods> InvoicesPaymentsMethods { get; set; }
        public ICollection<GlReciepts> Receipts { get; set; }


    }
}
