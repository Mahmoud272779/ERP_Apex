using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases
{
    public class SuppliersAccountRequest : GeneralPageSizeParameter
    {
        public bool zeroBalances { get; set; }
        public string Branches { get; set; }
        public bool IsSupplier { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
