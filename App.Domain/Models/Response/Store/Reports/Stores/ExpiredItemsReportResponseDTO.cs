using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Reports.Stores
{
    public class ExpiredItemsReportResponseDTO
    {
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }
        public string UnitNameAr { get; set; }
        public string UnitNameEn { get; set; }
        public double Quantity { get; set; }
        public DateTime ExpireDate { get; set; }

        //for print
        public string ExpireDateForPrint { get; set; }
    }
}
