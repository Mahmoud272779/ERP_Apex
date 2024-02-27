using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store
{
    public class ProfitRequest
    {
        public int  InvoiceId { get; set; }
        public double AVG { get; set; }
        public double Cost { get; set; }
        public int SizeId { get; set; }
        public int ItemId { get; set; }
    }

}
