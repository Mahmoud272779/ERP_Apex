using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvoicePaymentsMethods //جدول المسدد عن طريق طرق السداد المختلفة في فاتورة المشتريات
    {

        public int InvoicePaymentsMethodId { get; set; }
        public int PaymentMethodId { get; set; }
        public int InvoiceId { get; set; }
        public double Value { get; set; }
        public string Cheque { get; set; }
        public int BranchId { get; set; }
        public string CodeOfflinePOS { get; set; }

        public virtual InvoiceMaster InvoicesMaster { get; set; }
        public virtual InvPaymentMethods PaymentMethod { get; set; }

    }
}
