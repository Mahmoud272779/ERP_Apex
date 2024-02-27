using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores
{
    public class totalTransactionsOfItemsResponse
    {
        public double  totalPrevious { get; set; }
        public double totalInComingQuantity { get; set; }
        public double totalOutComingQuantity { get; set; }
        public double totalBalance { get; set; }
        public List<totalTransactionsOfItemsResponseList> detailsOfTransactions { get; set; }
    }
    public  class totalTransactionsOfItemsResponseList
    {
        public int itemId { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemNameEn { get; set; }
        public string unitName { get; set; }
        public string unitNameEn { get; set; }
        public double previous { get; set; }
        public double inComingQuantity { get; set; }
        public double outComingQuantity { get; set; }
        public double balance { get; set; }
    }

}
