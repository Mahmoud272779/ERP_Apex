using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Reports.Purchases
{
  public  class ItemsPurchasesResponse
    {
        public double SumQuantity { get; set; }
        public double SumTotal { get; set; }
        public double SumDiscount { get; set; }
        public double SumNet { get; set; }
        public double SumVat { get; set; }
        public double SumAvgPrice { get; set; }
        public double SumNetWithVat { get; set; }
        public List<ItemsPurchasesResponseList> Details { get; set; }
    }
    public class ItemsPurchasesResponseList
    {
      
        public int ItemId { get; set; }
        public string itemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }

       // public DateTime InvoiceDate { get; set; }
       // public int InvoiceTypeId { get; set; } // enum of invoiceType
       // public string InvoiceType { get; set; }
        public int? UnitId { get; set; }
        public string rptUnitAR { get; set; }
        public string rptUnitEn { get; set; }

        public double Quantity { get; set; }
        public double AvgPrice { get; set; }//سعر الشراء
        public double Total { get; set; }
        public double Discount { get; set; }
        public double Net { get; set; }
        public double Vat { get; set; }
        public double NetWithVat { get; set; }

    }
}
