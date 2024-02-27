using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Sales
{
    public class RPT_SalesProfitRequest
    {
        public DateTime DateFrom { get; set; }  
        public DateTime DateTo { get; set; }    
        public int PaymentType { get; set; }
        public int PageNumber { get; set; }
        public string branches { get; set; } = "";
        public int PageSize { get; set; }
    }
}
