using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.POS
{
    public class POSInvoiceSearchDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InvoiceTypeId { get; set; }
        public int? InvoiceCode { get; set; }
        public string? invoiceType { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? StoreId { get; set; }
        public int? PersonId { get; set; }
        public bool IsReturn { get; set; } = false;
        public int SessionId { get; set; } = 0;
    }
    public class POSReturnInvoiceSearchDTO : POSInvoiceSearchDTO
    {

        public InvoiceSearch? Searches { get; set; } = new InvoiceSearch();
    }
}
