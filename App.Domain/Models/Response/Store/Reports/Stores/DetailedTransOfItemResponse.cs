using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores
{
   public class DetailedTransOfItemResponse
    {
        public int invoiceId { get; set; }
        public DateTime  transDate { get; set; }
        public int invoiceTypeId { get; set; }
        public string documentId { get; set; }
        public string notes { get; set; }
        public double outComingQuantity { get; set; }
        public double inComingQuantity { get; set; }
        public double totalBalance { get; set; }


    }
}
