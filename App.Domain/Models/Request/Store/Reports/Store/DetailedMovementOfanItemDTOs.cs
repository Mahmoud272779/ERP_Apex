using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.returnAPISList;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class DetailedMovementOfanItemReqest
    {
        public int itemId { get; set; }
        public int unitId { get; set; }
        public int storeId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class InventoryValuationRequest 
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int storeId { get; set; }
        public int CategoryId { get; set; }
        public InventoryValuationType inventoryValuationType { get; set; }
        public int? itemId { get; set; }
        public DateTime? dateTo { get; set; }

    }
    public class DetailsOfSerialsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string serial { get; set; } 
    }
}
