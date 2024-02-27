using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class editedItemsParameter
    {
        public int itemId { get; set; }
        public int itemTypeId { get; set; }
        public int sizeId { get; set; }
        public double serialize { get; set; }
        public int invoiceType { get; set; }
        public int branchId { get; set; }
      //  public List<ItemsData> itemsData { get; set; } = new List<ItemsData>();
    }

    public class ItemsData
    {
   
    }
}
