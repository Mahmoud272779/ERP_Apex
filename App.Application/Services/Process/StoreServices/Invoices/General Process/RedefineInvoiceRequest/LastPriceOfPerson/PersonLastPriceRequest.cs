using App.Domain.Models.Security.Authentication.Request.Reports;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public class PersonLastPriceRequest
    {
        public int personId { get; set; }
        public int invoiceTypeId { get; set; }
        public List<InvoiceDetailsRequest> invoiceDetails { get; set; }
    }
  
}
