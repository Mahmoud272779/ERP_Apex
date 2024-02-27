using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Purchases
{
    public class VATDetailedReportRequest
    {
        public invoicesType invoicesType { get; set; }
        public string branchId { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public bool showInvoicesNotAllowedVat { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
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
