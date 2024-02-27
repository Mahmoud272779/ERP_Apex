using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.Vat.GetTotalVatData
{
    public class GetTotalVatDataRequest 
    {
        //public bool prevBalance { get; set; }
        public invoicesType invoicesType { get; set; }// = invoicesType.all;

        public int InvoiceType { get; set; }
        public string branchId { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public bool showInvoicesNotAllowedVat { get; set; }
    }
    public enum invoicesType
    {
        all = 1,
        Purchases,
        sales,
        returnPurchases,
        returnSales
    }
}
