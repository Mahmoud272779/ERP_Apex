using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores
{
    public class DetailedTransOfItemRequest
    {
        public int itemId { get; set; }
        public int unitId { get; set; }
        public int storeId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}
