using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores
{
    public class totalTransactionsOfItemsRequest
    {
        public int storeId { get; set; }
        public int itemId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
