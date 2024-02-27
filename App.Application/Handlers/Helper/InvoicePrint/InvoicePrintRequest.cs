using DocumentFormat.OpenXml.Presentation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.InvoicePrint
{
    public class InvoicePrintRequest : IRequest<ReportsReponse>
    {
        public int invoiceId { get; set; }
        public int screenId { get; set; }
        public string invoiceCode { get; set; }
        public exportType exportType { get; set; }
        public bool isPOS { get; set; } = false;
        public string employeeNameAr { get; set; }
        public string employeeNameEn { get; set; }
        public bool isPriceOffer { get; set; } 
        public bool isArabic { get; set; }

    }
}
