using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases
{
    public class purchasesTransactionRequest:GeneralPageSizeParameter
    {
        public int? supplierId { get; set; }
        public string branches { get; set; }
        public int? paymentMethod { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}
