using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class HomeResponse
    {
        public CountOfPurchasesInvoice countOfPurchasesInvoice { get; set; }
        public CountOfSalesInvoices countOfSalesInvoices { get; set; }
        public List<NewestInvoices> newestInvoices { get; set; }
        public InvoicesMovement invoicesMovement { get; set; }

        //---NEW----//
        public CountOfPurchasesWithoutVATInvoice countOfPurchasesWithoutVATInvoice { get; set; }
        public CountOfPOSInvoices CountOfPOSInvoices { get; set; }
        public purchasesAmout purchasesAmout { get; set; }
        public salesAmout salesAmout { get; set; }
        public HomeDataResponse_incomingAndOutgoingTransaction HomeDataResponse_incomingAndOutgoingTransaction { get; set; }

    }
    public class CountOfPurchasesInvoice
    {
        public int PaidInvoicesCount { get; set; }
        public int PartInvoicesCount { get; set; }
        public int DeportedInvoicesCount { get; set; }
    }
    public class CountOfPurchasesWithoutVATInvoice
    {
        public int PaidInvoicesCount { get; set; }
        public int PartInvoicesCount { get; set; }
        public int DeportedInvoicesCount { get; set; }
    }
    public class CountOfSalesInvoices
    {
        public int PaidInvoicesCount { get; set; }
        public int PartInvoicesCount { get; set; }
        public int DeportedInvoicesCount { get; set; }
    }
    public class CountOfPOSInvoices
    {
        public int PaidInvoicesCount { get; set; }
        public int PartInvoicesCount { get; set; }
        public int DeportedInvoicesCount { get; set; }
    }
    
    public class NewestInvoices
    {
        public int Id { get; set; }
        public int invoiceTypeId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string invoiceCode { get; set; }

    }
    public class InvoicesMovementDetalies
    {
        public int index { get; set; }
        public double SalesCount { get; set; }
        public double PurchasesCount { get; set; }
        public string date { get; set; }

    }

    public class InvoicesMovement
    {
        public double maximumValue { get; set; }
        public List<InvoicesMovementDetalies> data { get; set; }
    }
    public class salesAmout
    {
        public double percent { get; set; }
        public double currentMonth { get; set; }
        public double lastMonth { get; set; }
        public int monthDays { get; set; }
        public int currentDay { get; set; }
    }
    public class purchasesAmout
    {
        public double percent { get; set; }
        public double currentMonth { get; set; }
        public double lastMonth { get; set; }
        public int monthDays { get; set; }
        public int currentDay { get; set; }
    }
    public class HistoryMovement
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string ArabicTransactionType { get; set; }
        public string LatinTransactionType { get; set; }
        public DateTime DateTime { get; set; }

    }
    public class HomeDataResponse_incomingAndOutgoingTransaction
    {
        public double maximumValue { get; set; }
        public List<HomeDataResponse_incomingAndOutgoingTransaction_Detalies> incomingAndOutgoingTransactionDetalies { get; set; }
    }
    public class HomeDataResponse_incomingAndOutgoingTransaction_Detalies
    {
        public int index { get; set; }
        public double incoming { get; set; }
        public double outgoing { get; set; }
        public string date { get; set; }
    }
}
