using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request
{
    public class DirectPrintRequstDTO
    {
        public InvoiceDTO invoiceDto { get; set; }
        public exportType type { get; set; }
        public string printerName { get; set; }

    }
}
