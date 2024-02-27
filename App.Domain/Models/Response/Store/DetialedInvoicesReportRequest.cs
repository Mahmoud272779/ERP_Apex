using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store
{
    public class DetialedInvoicesReportRequest
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int userId { get; set; }
        public int invoiceTypeId { get; set; }
        public string documentCode { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int paymentMethod { get; set; }
        public string branches { get; set; }

    }
}
