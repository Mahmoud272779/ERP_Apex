using App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById;
using App.Application.Handlers.Invoices.Vat.GetTotalVatData;
using App.Application.Handlers.Reports;
using App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Request.Store.Reports.Sales;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Request.Store.Sales;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Models.Response.Store.Reports.Sales;

namespace App.Application.Services.Reports.StoreReports.Sales
{
    public interface iRPT_Sales
    {
        #region SalesOfCasher
        public Task<salesOfCasherResponse> GetSalesOfCasher(salesOfCasherDTO parm, bool isNotAccredit,bool isPrint = false); // ==> use for print
        public Task<ResponseResult> SalesOfCasher(salesOfCasherDTO parm, bool isNotAccredit,bool isPrint = false);// ==> use for website
        #endregion

        #region salesTransaction
        public Task<salesTransactionResponse> GetsalesTransaction(salesTransactionRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> salesTransaction(salesTransactionRequestDTO parm, bool isPrint = false);
        #endregion

        #region itemsSales
        public Task<ResponseResult> itemsSales(ItemSalesRequestDto parm, bool isPrint = false);//<=== use this service for showing the report in website
        public Task<itemsSalesReponse> GetItemsSales(ItemSalesRequestDto parm, bool isPrint = false); //<=== use this service for printing
        #endregion

        #region Total Sales Of Branch
        public Task<totalSalesOfBranchesResponse> getTotalSalesOfBranch(totalSalesOfBranchesRequestDTO parm,bool isPrint = false);
        public Task<ResponseResult> TotalSalesOfBranch(totalSalesOfBranchesRequestDTO parm);

        #endregion

        #region Item Sales For Customers 
        public Task<itemSalesForCustomersResponse> getItemSalesForCustomers(itemSalesForCustomersRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> ItemSalesForCustomers(itemSalesForCustomersRequestDTO parm);
        #endregion

        #region items not sold 
        public Task<itemsNotSoldResponse> getItemsNotSold(itemsNotSoldRequstDTO parm, bool isPrint = false);
        public Task<ResponseResult> ItemsNotSold(itemsNotSoldRequstDTO parm);
        #endregion

        #region sales And Sales Return Transaction 
        public Task<salesAndSalesReturnTransactionResponse> getsalesAndSalesReturnTransaction(salesAndSalesReturnTransactionRequstDTO parm, bool isPrint = false);
        public Task<ResponseResult> salesAndSalesReturnTransaction(salesAndSalesReturnTransactionRequstDTO parm);
        #endregion

        #region Items Most Sales 
        public Task<itemsSoldMostResponse> getitemsSoldMost(itemsSoldMostRequstDTO parm, bool fromDashboard = false, bool isPrint = false);
        public Task<ResponseResult> itemsSoldMost(itemsSoldMostRequstDTO parm);
        #endregion

        #region total Branch Transaction
        public Task<totalBranchTransactionResponse> getTotalBranchTransaction(totalBranchTransactionRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> totalBranchTransaction(totalBranchTransactionRequestDTO parm);
        #endregion

        Task<WebReport> GetSalesOfCasherReport(salesOfCasherDTO request, bool isNotAccredit, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> itemsSalesReport(ItemSalesRequestDto param, exportType exportType, bool iaArabic, int fileId = 0);
        Task<WebReport> SalesTransactionReport(salesTransactionRequestDTO parm, int screenId, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> ItemsSoldMostReport(itemsSoldMostRequstDTO parm, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> ItemsNotSoldReport(itemsNotSoldRequstDTO parm, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> ItemSalesForCustomersReport(itemSalesForCustomersRequestDTO request, exportType exportType, bool isArabic, int fileId = 0);

        Task<WebReport> salesAndSalesReturnTransactionReport(salesAndSalesReturnTransactionRequstDTO request, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> TotalSalesOfBranchReport(totalSalesOfBranchesRequestDTO request, exportType exportType, bool isArabic,string expnadedBranchId, int fileId = 0);
        Task<WebReport> TotalBranchTransactionReport(totalBranchTransactionRequestDTO request, exportType exportType, bool isArabic, string expandedTypeId, int fileId = 0);

        Task<WebReport> PriceOfferReport(GetOfferPriceByIdRequest param, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> SalesOfSalesManReport(SalesOfSalesManRequest param, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> ItemsProfitReport(ItemsProfitRequest request, exportType exportType, bool isArabic, int fileId = 0);
     
        Task<ResponseResult> GetDebtAgingForCustomresOrSuplier(DebtAgingForCustomersOrSuppliersRequest request);
        Task<WebReport> DebtAgingForCustomresOrSuplierReport(DebtAgingForCustomersOrSuppliersRequest request, exportType exportType, bool isArabic, int fileId = 0);
        Task<ResponseResult> GetDetailedInvoicesReport(DetialedInvoicesReportRequest parameter, bool isPrint = false);
        Task<WebReport> DetailedInvoicesPrint(DetialedInvoicesReportRequest parameter, exportType exportType, bool isArabic, int fileId = 0);
    }
}
