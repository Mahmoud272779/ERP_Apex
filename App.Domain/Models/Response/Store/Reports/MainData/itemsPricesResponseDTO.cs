using App.Domain.Models.Response.Store.Reports.Sales;
using App.Domain.Models.Setup.ItemCard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.MainData
{
    public class itemsPricesResponseList: InvItemCardDTO
    {
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double purchasesPrice { get; set; }
        public double salesPrice1 { get; set; }
        public double salesPrice2 { get; set; }
        public double salesPrice3 { get; set; }
        public double salesPrice4 { get; set; }
    }
    public class itemsPricesResponse
    {
        public List<itemsPricesResponseList> data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int TotalCount { get; set; }
        public int dataCount { get; set; }
    }
}
