using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.PurchasesDtos
{
    public class InvoiceSearchPagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
      //  public int BranchId { get; set; } = 1;
        public int InvoiceTypeId { get; set; } // for purchase and return purchase

        public InvoiceSearch Searches { get; set; } = new InvoiceSearch();
    }
        public class InvoiceSearch
        {
        public string SearchCriteria { get; set; }
        public int[] PaymentType { get; set; }
        public int[] InvoiceTypeId { get; set; } // enum of invoiceType
        public int[] SubType { get; set; } // مرتجع جزئي و كلى
        public int[] StoreId { get; set; }
        public int?[] PersonId { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public int? itemId { get; set; } = 0;
        public int? categoryId { get; set; } = 0;
        public bool isExpenses { get; set; } = false;
    }
}
