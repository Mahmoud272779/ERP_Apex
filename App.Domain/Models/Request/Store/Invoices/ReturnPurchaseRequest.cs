using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Invoices
{
    public class ReturnPurchaseSearchPagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public ReturnPurchaseRequest Searches { get; set; } = new ReturnPurchaseRequest();
    }

    public class ReturnPurchaseRequest 
    {
        public int[] PaymentType { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
    }
}
