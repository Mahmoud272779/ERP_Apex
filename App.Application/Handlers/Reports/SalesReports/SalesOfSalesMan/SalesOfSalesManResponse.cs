using App.Domain.Models.Response.Store.Reports.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan
{
    public class SalesOfSalesManResponse
    {
        public bool isCustomer { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceDate { get; set; }
        public DateTime FullInvoiceDate { get; set; }
        public int DocumenTypeID { get; set; }
        public int paymentType { get; set; }
        public int PaymentCount { get; set; }
        public string CustomerNameAr { get; set; } //اسم العميل
        public string CustomerNameEn { get; set; }//
        public string PaymentTypeNameAr { get; set; }// طرق السداد 
        public string PaymentTypeNameEn { get; set; }//طرق السداد
        public double Net { get; set; }//الصافى
        public double TotalPrice { get; set; }//الاجمالى
        public double Paid { get; set; }//المدفوع 
        public double discount { get; set; }//الخصم
        public double Cost { get; set; }//االتكلفه
        public double Vat { get; set; }// القيمة المضافة
        public double Profit { get; set; } //الربحيه
        public string rowClassName { get; set; }

        public int InvoiceId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PageEnum { get; set; }
        public int PageId { get; set; }
        public int DocTypeId { get; set; }
    }
    public class TotalsOfSalesManData
    {
        public List<SalesOfSalesManResponse> _SalesOfSalesManResponseList;
        public double TotalDiscount { get; set; }
        public double TotalPaid { get; set; }
        public double TotalOfTotalPrice { get; set; }
        public double TotalNet { get; set; }
        public double TotalProfit { get; set; }
        public double TotalVat { get; set; }
    }
}
