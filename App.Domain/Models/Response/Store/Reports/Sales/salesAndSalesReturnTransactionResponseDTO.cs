using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class salesAndSalesReturnTransactionResponseDTO
    {
        public double TotalQyt { get; set; }
        public double TotalPrice { get; set; }
        public double Total { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalNet { get; set; }

        public List<salesAndSalesReturnTransactionResponseList> list { get; set; }
    }
    public class salesAndSalesReturnTransactionResponseList
    {
        public string date { get; set; }
        public string invoiceCode { get; set; }
        public string invoiceTypeAr { get; set; }
        public string invoiceTypeEn { get; set; }
        public string itemArabicName { get; set; }
        public string itemLatinName { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double qyt { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public double discount { get; set; }
        public double net { get; set; }
        public string rowClassName { get; set; }
        // for print
        public string ItemCode { get; set; }
        
    }
    public class salesAndSalesReturnTransactionResponse
    {
        public salesAndSalesReturnTransactionResponseDTO data { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
    }
}
