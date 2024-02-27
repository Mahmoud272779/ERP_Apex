using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public class ReportOtherData
    {
        public int? Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }

        public string Code { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameEn { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public DateTime ReportDate { get; set; }
        public string Date { get; set; }
      // public string  InvoiceTypeReport { get; set; }
        public string Currency { get; set; }

    }
}
