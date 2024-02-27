using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases
{
   public class ItemsPurchasesRequest:GeneralPageSizeParameter        
    {
        public int? paymentMethod { get; set; }
        public int? itemId { get; set; } = 0;
        public int? itemType { get; set; }
        public int? categoryId { get; set; } = 0;
        public string Branches { get; set; }
        public int? storeId { get; set; } = 0;
        public int? supplierId { get; set; } = 0;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
