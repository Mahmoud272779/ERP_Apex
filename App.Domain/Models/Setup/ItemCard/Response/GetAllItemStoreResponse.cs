using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Response
{
    public class GetAllItemStoreResponse
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal DemandLimit { get; set; }
        public virtual InvStpItemCardMaster Item { get; set; }
        public virtual InvStpStores Store { get; set; }
    }
}
