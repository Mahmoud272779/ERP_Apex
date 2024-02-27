using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Setup
{
    public class InvStpItemCardSerials
    {
        public int ItemId { get; set; }
        public string SerialNo { get; set; }
        public virtual InvStpItemCardMaster Item { get; set; }
    }
}
