using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store
{
    public  class InvPersonLastPrice
    {
        public int id { get; set; }
        public int personId { get; set; }
        public int itemId { get; set; }
        public int unitId { get; set; }
        public int invoiceTypeId { get; set; }
        public double price { get; set; }

    }
}
