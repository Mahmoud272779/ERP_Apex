using App.Domain.Entities.Process;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemCardStores
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal DemandLimit { get; set; }
       // public bool WillDelete { get; set; }
        public virtual InvStpItemCardMaster Item { get; set; }
        public virtual InvStpStores Store { get; set; }
    }
}