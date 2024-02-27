using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store
{
    public class TotalAccrediteInvoiceResponse
    {
        public List<AccrediteInvoiceResponseData> accrediteInvoiceResponseData { get; set; }
        public List<PaymentMethodsDataResponse> paymentMethodType { get; set; }

    }
    public class AccrediteInvoiceResponseData
    {
        public DateTime InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public int InvoiceTypeId { get; set; }
        public string transactionTypeAr { get; set; }
        public string transactionTypeEn { get; set; }
        public double Net { get; set; } 
        public double Paied { get; set; }
        public bool isCustomer { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        //for  print
        public int InvoiceId { get; set; }
       
       
        public int PageEnum { get; set; }
        public int PageId { get; set; }
        public int DocTypeId { get; set; }
        public double totalOtherPaymentMethodsValues { get; set; }
        public List<PaymentMethodsDataResponse> paymentMethods { get; set; }
        public List<PaymentMethodsDataResponse> paymentMethodType { get; set; }=new List<PaymentMethodsDataResponse>();
    }
    public class PaymentMethodsDataResponse
    {
        public int Count { get; set; }
        public double Value { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }

        //for print
        public int PaymentMethodId { get; set; }

        public int InvoiceId { get; set; }
    }

    public class SafeOrBankDataResponse

    {
       // public int Count { get; set; }
        public double Value { get; set; }
        public string safeOrBankNameAr { get; set; }
        public string safeOrBankNameEn { get; set; }
        public string InvoiceType { get; set; }
    }
    public class FinalPaymentMethodsDataResponse

    {
      public  List<PaymentMethodsDataResponse> InvoiceDetails { get; set; } 
       public List<PaymentMethodsDataResponse> ReturnInvoiceDetails { get; set; }
    }
    public class FinalBankOrSafeDataResponse

    {
        public List<SafeOrBankDataResponse> InvoiceDetails { get; set; }
        public List<SafeOrBankDataResponse> ReturnInvoiceDetails { get; set; }
    }

    public class PaymentMethodsForPrint
    {
        public double TotalCash { get; set; }
        public double TotalNet { get; set; }
        public double Other { get; set; }
        public int InvoiveId { get; set; }

    }

}
