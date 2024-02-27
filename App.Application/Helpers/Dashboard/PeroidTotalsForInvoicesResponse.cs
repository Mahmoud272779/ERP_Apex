using App.Domain.Models.Response.Store.Reports.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Dashboard
{
    public class PeroidTotalsForInvoicesResponse
    {
        public TotalCurrentSales totalCurrentSales { get; set; }
        public TotalCurrentPurchases totalCurrentPurchaes { get; set; }

    }
    public class TotalsForInvoices
    {
        public double totalRemian { get; set; }
        public double totalPaid { get; set; }
    }
    public class TotalCurrentSales : TotalsForInvoices
    {
        public double TotalSales { get; set; }
    }
    public class TotalCurrentPurchases : TotalsForInvoices
    {
        public double TotalPurchases { get; set; }
    }
    public class GetInvoices
    {
        public double net { get; set; }
        public double paid { get; set; }

        public int invoiceTypeId { get; set; }
        public DateTime InvoiceDate { get; set; }
    }

    public class IncomingCurrentPeroidResponse
    {
        public IncomingCurrentPeroid incomingCurrentPeroid { get; set; }
    }
    public class IncomingCurrentPeroid
    {
        public double incoming { get; set; }
        public double revenues { get; set; }

        public double expenses { get; set; }

    }

    public class BanksSafesBalancesReponse
    {
        public List<BankSafeBalances> banksBalances { get; set; }
        public List<BankSafeBalances> safesBalances { get; set; }

        public double totalBanksBalance { get; set; }
        public double totalSafesBalance { get; set; }

    }
    public class BankSafeBalances
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double balance { get; set; }



    }
    public class SalesTransactionResponse
    {
        public List<SalesTransaction> SalesTransaction { get; set; }
    }


    public class SalesTransaction : TotalCurrentSales
    {
        public int Month { get; set; }
    }
    public class SalesPurchasesTransaction
    {
        public int Day { get; set; }
        public double Net { get; set; }
    }
    public class SalesPurchasesTransactionRsponse
    {
        public List<SalesPurchasesTransaction> SalesTransaction { get; set; }
        public List<SalesPurchasesTransaction> PurchasesTransaction { get; set; }

    }
    public class ReceiptsResponse
    {
        public double Amount { get; set; }
        public int RecieptTypeId { get; set; }
        public DateTime ReceiptDate { get; set; }
    }
    public class RevenuesExpensesTransaction : SalesPurchasesTransaction
    {

    }

    public class RevenuesExpensesTransactionRsponse
    {
        public List<RevenuesExpensesTransaction> revenuesTransaction { get; set; }
        public List<RevenuesExpensesTransaction> expensesTransaction { get; set; }
    }
    public class FinancailFlow
    {
        public int Month { get; set; }
        public double revenues { get; set; }
        public double expenses { get; set; }

    }

    public class FinancailFlowResponse
    {
        public List<FinancailFlow> FinancailFlow { get; set; }
    }
    public class NewestInvoices
    {
        public int InvoiceTypeId { get; set; }
        public string CusomerSupplierNameAr { get; set; }
        public string CusomerSupplierNameEn { get; set; }
        public string InvoiceTypeAr { get; set; }
        public string InvoiceTypeArEn { get; set; }
        public double Net { get; set; }
        public int Status { get; set; }
        public string InvoiceCode { get; set; }

    }
    public class NewestInvoicesAndMostSoldItems
    {
        public List<NewestInvoices> NewestInvoices { get; set; }
        public List<itemsSoldMostResponseList> ItemsSoldMost { get; set; }
    }
    public class SalesMenWhoSoldMost
    {
        public string SalesManNameAr { get; set; }
        public string SalesManNameEn { get; set; }

        public string BranchesAr { get; set; }
        public string BranchesEn { get; set; }

        public int InvoicesCount { get; set; }

        public double TotalAmountOfInvoices { get; set; }
        public int salesManId { get;set; }


    }
    public class SalesMenWhoSoldMostResponse
    {
        public List<SalesMenWhoSoldMost> SalesMenWhoSoldMost { get; set; }
    }
}

