using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Reports.Purchases
{
    public class ItemPurchasesForSupplierResponse
    {
        public double SumQuantity { get; set; }
        public double SumTotal { get; set; }
        public double SumDiscount { get; set; }
        public double SumNet { get; set; }
        public double SumVat { get; set; }
        public double SumAvgPrice { get; set; }

        public double SumNetWithVat { get; set; }
      
        public List<ItemPurchasesForSupplierResponseList> Details { get; set; }
    }
    public class ItemPurchasesForSupplierResponseList
    {
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceTypeId { get; set; } // enum of invoiceType
        public string TransactionAr { get; set; }
        public string TransactionEn { get; set; }
        public string InvoiceType { get; set; }
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

        //for print
        public string Date { get; set; }

    }
   
}
