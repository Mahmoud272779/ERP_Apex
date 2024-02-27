using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class itemsSalesReponseDTO
    {
        public double TotalQyt { get; set; }
        public double TotalAavgOfPrice { get; set; }
        public double TotalPrice { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalNet { get; set; }
        public List<itemsSalesData> data { get; set; }
    }
    public class itemsSalesData
    {
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double qyt { get; set; }
        public double avgOfPrice { get; set; }
        public double totalPrice { get; set; }
        public double discount { get; set; }
        public double net { get; set; }
    }
    public class ConvertitemsSalesData
    {
        public int itemId { get; set; }
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double qyt { get; set; }
        public double price { get; set; }
        public double discount { get; set; }
        public double net { get; set; }
    }
    public class itemsSalesReponse
    {
        public itemsSalesReponseDTO data { get; set; }
        public string notes { get; set; }
        public int TotalCount { get; set; }
        public int dataCount { get; set; }
        public Result Result { get; set; }
    }
}
