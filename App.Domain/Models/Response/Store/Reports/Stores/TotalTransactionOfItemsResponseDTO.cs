using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Stores
{
    public class TotalTransactionOfItemsResponseDTO
    {
        public double TotalPrevious { get; set; }
        public double TotalIncoming { get; set; }
        public double TotalOutgoing { get; set; }
        public double TotalBalance { get; set; }
        public List<TotalTransactionOfItemsList> data { get; set; }
    }

    public class TotalTransactionOfItemsList
    {
        public int itemId { get; set; }
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string unitNameAr { get; set; }
        public string unitNameEn { get; set; }
        public double previous { get; set; }
        public double incoming { get; set; }
        public double outgoing { get; set; }
        public double balance { get; set; }
    }
    public class TotalTransactionOfItemsResponse
    {
        public TotalTransactionOfItemsResponseDTO data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
    }
}
