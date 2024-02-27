using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class itemsSoldMostResponseDTO
    {
        public double TotalQyt          { get; set; }
        public double TotalAvgPrice     { get; set; }
        public double TotalCost         { get; set; }
        public double TotalDiscount     { get; set; }
        public double TotalVat          { get; set; }
        public double TotalNet          { get; set; }
        public List<itemsSoldMostResponseList> itemsSoldMostResponseLists { get; set; }
    }
    public class itemsSoldMostResponseList
    {
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double Qyt { get; set; }
        public double AvgPrice { get; set; }
        public double Cost { get; set; }
        public double Discount { get; set; }
        public double Vat { get; set; }
        public double Net { get; set; }
        public double NetWithVat { get; set; }
    }
    public class itemsSoldMostResponse
    {
        public itemsSoldMostResponseDTO data { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public Result Result { get; set; }
        public string notes { get; set; }
    }
}
