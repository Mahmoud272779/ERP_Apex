using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases
{
    public class purchasesTransactionResponse
    {
        public double  totalPurchasesNet { get; set; }
        public double  totalReturnPurchasesNet { get; set; }
        public double  totalNet { get; set; }
        public List<purchasesTransactionList> purchasesTransactionList { get; set; }
    }

    public class purchasesTransactionList
    {
        public int? supplierId { get; set; }
        public string supplierName { get; set; }
        public string supplierNameEn { get; set; }

        public int paymentMethodId { get; set; }
        public string paymentMethod { get; set; }
        public string paymentMethodEn { get; set; }

        public int purchasesCount { get; set; }
        public double purchasesNet { get; set; }
        public int returnPurchasesCount { get; set; }
        public double returnPurchasesNet { get; set; }
        public double Net { get; set; }

    }
}
