using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemCardParts
    {
       // public int Id { get; set; }
        public int ItemId { get; set; }
        public int PartId { get; set; }
        public double Quantity { get; set; }
        public int UnitId { get; set; }
      //  public bool WillDelete { get; set; }

        public virtual InvStpItemCardMaster CardMaster { get; set; }
        public virtual InvStpItemCardMaster PartDetails { get; set; }
        public virtual InvStpUnits Unit { get; set; }
    }
}
