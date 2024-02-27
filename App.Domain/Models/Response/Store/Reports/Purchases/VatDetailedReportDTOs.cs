using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Reports.Purchases
{
    public class VatDetailedReportResponse
    {
        
        public List<data> items { get; set; }
        public double Net { get; set; }
        public double TotalHasVat { get; set; }
        public double VatValue { get; set; }
        public double TotalAfterVat { get; set; }
    }
    public class data
    {
        public string invoiceCode { get; set; }
        public DateTime invoiceDate { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string PersonVatNumber { get; set; }
        public double net { get; set; }
        public double totalHasVat { get; set; }
        public double vatValue { get; set; }
        public double totalAfterVat { get; set; }

        public int? InvoiceId { get; set; }
        public int? InvoiceTypeId { get; set; }

        public int? pageId { get; set; }
        public int? pageEnum { get; set; }
        public int? DocTypeId { get; set; }

    }
    public class PreviousBalances
    {
        public double previousNet { get; set; }
        public double previousTotalHasVat { get; set; }
        public double previousVatValue { get; set; }
        public double previousTotalAfterVat { get; set; }

    }
}
