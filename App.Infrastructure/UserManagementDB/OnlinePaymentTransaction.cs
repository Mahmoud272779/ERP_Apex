using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class OnlinePaymentTransaction
    {
        public int Id { get; set; }
        public int CashTransactionId { get; set; }
        public string TotalWithFormat { get; set; }
        public string PaymentInvoiceId { get; set; }
        public string PaymentUrl { get; set; }
        public string InvoiceStatues { get; set; }
        public string OnlinePaymentId { get; set; }
        public string Last4DigitsOfVisaCard { get; set; }
        public string VisaCardOwnerName { get; set; }
        public int? UserApplicationCashId { get; set; }

        public virtual UserApplicationCash UserApplicationCash { get; set; }
    }
}
