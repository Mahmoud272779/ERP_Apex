using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Invoices
{
    public class QuantityDto
    {
        public int itemId { get; set; }
        public int sizeId { get; set; }
        public string itemCode { get; set; }
        public double qty { get; set; }
        public DateTime? expiryDate { get; set; }

    }
}
