using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases
{
    public class SupplierAccountRequest : GeneralPageSizeParameter
    {
        public bool PaidPurchase { get; set; }
        public int personId { get; set; }
        public  int[] Branches { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
