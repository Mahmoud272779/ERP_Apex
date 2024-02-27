using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Invoices
{
    public class StoreSearchPagination
    {
        public int PageNumber { get; set; }  
        public int PageSize { get; set; }
        //public int BranchId { get; set; }
        public StoreSearch Searches { get; set; } = new StoreSearch();
    }
    public  class StoreSearch
    {
        public string SearchCriteria { get; set; }
        public int[] StoreId { get; set; }
        public int[] InvoiceTypeId { get; set; } // enum of invoiceType

        // public int[] ItemId { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public int? itemId { get; set; } = 0;
        public int? categoryId { get; set; } = 0;
    }
}
