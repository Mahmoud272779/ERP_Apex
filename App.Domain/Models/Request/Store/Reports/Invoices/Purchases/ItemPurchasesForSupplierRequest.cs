using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases
{
    public class ItemPurchasesForSupplierRequest :GeneralPageSizeParameter
    {
        [Required]
        public int personId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string Branches { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
    }
}
