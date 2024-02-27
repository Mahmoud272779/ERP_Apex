using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases
{
    public class ItemsPurchasesForSupplierRequest:GeneralPageSizeParameter
    {
        [Required]
        public int personId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PaymentTypeId { get; set; }
        [Required]
        public string Branches { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
    }
}
