using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public  class RPT_SalesProfitResponse
    {
        public bool isCustomer { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceDate { get; set; }
        public DateTime FullInvoiceDate { get; set; }
        public string DocumenTypeAr { get; set; }
        public int DocumenTypeID{ get; set; }
        public int paymentTypeId{ get; set; }
        public int PaymentCount{ get; set; }
        public string DocumenTypeEn { get; set; }
        public string CustomerNameAr { get; set; }
        public string CustomerNameEn { get; set; }
        public string PaymentTypeNameAr { get; set; }
        public string PaymentTypeNameEn { get; set; }
        public double Net { get; set; }
        public double Cost { get; set; }
        public double Profit { get; set; }
        public string rowClassName { get; set; }

        public int InvoiceId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PageEnum { get; set; }
        public int PageId { get; set; }
        public int DocTypeId { get; set; }

    }

    public class TotalRPT_SalesProfitResponse
    {
       public List<RPT_SalesProfitResponse> _salesProfitResponseList;
        public double TotalCost { get; set; }
        public double TotalNet { get; set; }
        public double TotalProfit { get; set; }

    }
}
