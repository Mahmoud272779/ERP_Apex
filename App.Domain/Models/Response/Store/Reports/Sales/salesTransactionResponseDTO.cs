using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class salesTransactionResponseDTO
    {
        public double totalAmount { get; set; }
        public double totalDiscount { get; set; }
        public double totalAfterDiscount { get; set; }
        public double totalVat { get; set; }
        public double net { get; set; }
        public double totalPaid { get; set; }
        public double totalRemin { get; set; }
        public List<SalesTransactionDetalies> data { get; set; }
    }
    public class SalesTransactionDetalies
    {
        public bool isCustomer { get; set; }
        public string documentCode { get; set; }
        public DateTime date { get; set; }
        public string documentTypeAr { get; set; }
        public string documentTypeEn { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public string rowClassName { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }
        public double totalAfterDiscount { get; set; }
        public double vat { get; set; }
        public double net { get; set; }
        public double paid { get; set; }
        public double remin { get; set; } 
        //for print
        public string DocumentDate { get; set; }
        public string DocumentTime { get; set; }
        public bool? isExpenses { get; set; }// مصروفات

        public int invoiceTypeId { get; set; }
        public int InvoiceId { get; set; }
        public int pageEnum { get; set; }
        public int pageId { get; set; }
        public int DocTypeId { get; set; }
    }
    public class salesTransactionResponse
    {
        public salesTransactionResponseDTO data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int TotalCount { get; set; }
        public int dataCount { get; set; }
    }
}
