using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases
{
    public class VatStatmentRequest : GeneralPageSizeParameter
    {
        public bool prevBalance { get; set; }
        public string branches { get; set; }
        public int? InvoiceType { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }

       
    }


}
