using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier;
using App.Application.Services.Reports.Invoices.Purchases.Supplier_Account;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Request.Store.Reports.Sales;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan;
using App.Application.Handlers.Reports;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Models.Request.Store.Sales;
using App.Domain.Models.Response.Store;
using App.Application.Handlers.Profit.profitPages;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class reportsOfSalesController : ApiStoreControllerBase
    {
        private readonly IPersonAccountService _supplierAccountService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IPersonHelperService _personHelperService;
        private readonly iRPT_Sales _iRPT_Sales;
        private readonly IMediator _mediator;

        private readonly IItemPurchasesForSupplierService _itemPurchasesForSupplierService;
        private readonly IItemsPurchasesForSupplierService _itemsPurchasesForSupplierService;
        private readonly ISuppliersAccountService _SuppliersAccountService;
        private readonly IprintFileService _iPrintFileService;
        private readonly IVatStatementService _vatStatementService;
        WebReport report = new WebReport();

        
        public reportsOfSalesController(IMediator mediator,
                                        IPersonAccountService supplierAccountService,
                                        iAuthorizationService iAuthorizationService,
                                        IActionResultResponseHandler ResponseHandler,
                                        IPersonHelperService personHelperService,
                                        iRPT_Sales iRPT_Sales,
                                        IItemPurchasesForSupplierService itemPurchasesForSupplierService,
                                        IItemsPurchasesForSupplierService itemsPurchasesForSupplierService,
                                        ISuppliersAccountService SuppliersAccountService,
                                        IVatStatementService VatStatmentServices,
                                        IprintFileService iPrintFileService,
                                        IVatStatementService vatStatementService) : base(ResponseHandler)
        {
            _supplierAccountService = supplierAccountService;
            _iAuthorizationService = iAuthorizationService;
            _personHelperService = personHelperService;
            _iRPT_Sales = iRPT_Sales;
            _itemPurchasesForSupplierService = itemPurchasesForSupplierService;
            _itemsPurchasesForSupplierService = itemsPurchasesForSupplierService;
            _SuppliersAccountService = SuppliersAccountService;
            _iPrintFileService = iPrintFileService;
            _mediator = mediator;
            _vatStatementService = vatStatementService;
        }

        [HttpGet("GetCustomerAccountData")]
        public async Task<ResponseResult> GetCustomerAccountData(int customerId, string? branches, DateTime dateFrom, DateTime dateTo, bool PaidPurchase, int PageSize, int pageNumber)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.CustomerStatement_Sales, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            int[] branchesIds = null;
            if (!string.IsNullOrEmpty(branches))
                branchesIds = Array.ConvertAll(branches.Split(','), s => int.Parse(s));
            if (_personHelperService.checkIsCustomer(customerId).Result)
                return new ResponseResult()
                {
                    Note = "customerId is wrong"
                };

            
            SupplierAccountRequest request = new SupplierAccountRequest()
            {
                personId = customerId,
                PaidPurchase = PaidPurchase,
                Branches = branchesIds,
                DateFrom = dateFrom,
                DateTo = dateTo,
                PageSize = PageSize,
                PageNumber = pageNumber
            };


            var result = await _supplierAccountService.GetPersonAccountData(request);
            return result;
        }

        [HttpGet("SalesOfCasher")]
        public async Task<ResponseResult> SalesOfCasher([FromQuery]salesOfCasherDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.salesOfCasher, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.SalesOfCasher( parm,false );
        }
        [HttpGet("SalesOfCasherReport")]
        public async Task<IActionResult> SalesOfCasherReport([FromQuery] salesOfCasherDTO parm,bool isNotAccridet,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.salesOfCasher, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            report= await _iRPT_Sales.GetSalesOfCasherReport(parm, isNotAccridet, exportType, isArabic,fileId);

            string ReportFileName = null;
            if (!isNotAccridet)
            {
                ReportFileName = "salesOfCasherAccredit";

            }
            else
            {
                ReportFileName = "salesOfCasherNotAccredit";

            }

            return Ok( _iPrintFileService.PrintFile(report, ReportFileName, exportType));

        }
        [HttpGet("SalesOfCasherNotAccredit")]
        public async Task<ResponseResult> SalesOfCasherNotAccredit([FromQuery] salesOfCasherDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.salesOfCasherNotAccredit, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.SalesOfCasher(parm, true);
        }
        [HttpGet("GetCustomerBalances")]
        public async Task<ResponseResult> GetCustomerBalances([FromQuery] string branches, DateTime dateFrom, DateTime dateTo, bool zeroBalances, int PageSize, int pageNumber)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.CustomersBalances, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            SuppliersAccountRequest request = new SuppliersAccountRequest()
            {
                zeroBalances = zeroBalances,
                Branches = branches,
                DateFrom = dateFrom,
                DateTo = dateTo,
                PageSize = PageSize,
                PageNumber = pageNumber,
                IsSupplier = false
            };
            var result = await _SuppliersAccountService.GetSuppliersAccountData(request,true, false);
            return result;
        }
        [HttpGet("CustomerBalancesReport")]
        public async Task<IActionResult> CustomerBalancesReport([FromQuery] SuppliersAccountRequest request,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.CustomersBalances, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);

            request.IsSupplier = false;
            report = await _SuppliersAccountService.SuppliersCustomersBalancesReport(request,true, exportType, isArabic,fileId);

            return Ok(_iPrintFileService.PrintFile(report, "CustomersBalances", exportType));
        }
        [HttpGet("salesTransaction")]
        public async Task<ResponseResult> salesTransaction([FromQuery] salesTransactionRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesTransaction, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.salesTransaction(parm);
        }
        [HttpGet("SalesTransactionReport")]
        public async Task<IActionResult> SalesTransactionReport([FromQuery] salesTransactionRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesTransaction, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            //WebReport report = new WebReport();



            report = await _iRPT_Sales.SalesTransactionReport(parm, (int)SubFormsIds.SalesTransaction, exportType, isArabic, fileId);

            return Ok(_iPrintFileService.PrintFile(report, "SalesTransaction", exportType));


        }
        [HttpGet("itemsSales")]
        public async Task<ResponseResult> itemsSales([FromQuery] ItemSalesRequestDto parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.ItemSales, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.itemsSales(parm);
        }
        [HttpGet("itemsSalesReport")]
        public async Task<IActionResult> itemsSalesReport([FromQuery] ItemSalesRequestDto parm,exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.ItemSales, Opretion.Print);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();
            report = await _iRPT_Sales.itemsSalesReport(parm,  exportType, isArabic, fileId);

            return Ok(_iPrintFileService.PrintFile(report, "ItemSales", exportType));

           
        }
        [HttpGet("getTotalSalesOfBranch")]
        public async Task<ResponseResult> TotalSalesOfBranch([FromQuery] totalSalesOfBranchesRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.TotalSalesOfBranch, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.TotalSalesOfBranch(parm);
        }
        [HttpGet("TotalSalesOfBranchReport")]
        public async Task<IActionResult> TotalSalesOfBranchReport([FromQuery] totalSalesOfBranchesRequestDTO parm,exportType exportType,bool isArabic,string? expandedBranchId, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.TotalSalesOfBranch, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();



            report = await _iRPT_Sales.TotalSalesOfBranchReport(parm, exportType, isArabic, expandedBranchId, fileId);

            return Ok(_iPrintFileService.PrintFile(report, "TotalSalesOfCompanyReport", exportType));
        }
        [HttpGet("GetItemSalesOfCustomer")]
        public async Task<ResponseResult> GetItemSalesOfCustomer([FromQuery] ItemPurchasesForSupplierRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemSalesForCustomer, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _itemPurchasesForSupplierService.GetItemPurchasesForSupplierData(parm);
        }
        [HttpGet("ItemSalesForCustomerReport")]
        public async Task<IActionResult> ItemSalesForCustomerReport([FromQuery] ItemPurchasesForSupplierRequest parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemSalesForCustomer, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            var report = new WebReport();
            report= await _itemPurchasesForSupplierService.ItemPurchasesForSupplierReport(parm,exportType,isArabic, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemSalesForCustomer", exportType));

        }
        [HttpGet("GetItemsSalesOfCustomer")]
        public async Task<ResponseResult> GetItemsSalesOfCustomer([FromQuery] ItemsPurchasesForSupplierRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSalesForCustomer, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _itemsPurchasesForSupplierService.GetItemsPurchasesForSupplierData(parm);
        }
        [HttpGet("ItemsSalesForCustomerReport")]
        public async Task<IActionResult> ItemsSalesForCustomerReport([FromQuery] ItemsPurchasesForSupplierRequest parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSalesForCustomer, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
           // return await _itemsPurchasesForSupplierService.GetItemsPurchasesForSupplierData(parm);

            var report = new WebReport();
            report = await _itemsPurchasesForSupplierService.ItemsPurchasesForSupplierReport(parm, exportType, isArabic, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemsSalesForCustomer", exportType));
        }
        [HttpGet("ItemSalesForCustomers")]
        public async Task<ResponseResult> ItemSalesForCustomers([FromQuery] itemSalesForCustomersRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemSalesForCustomers, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.ItemSalesForCustomers(parm);
        }
        [HttpGet("ItemSalesForCustomersReport")]
        public async Task<IActionResult> ItemSalesForCustomersReport([FromQuery] itemSalesForCustomersRequestDTO parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemSalesForCustomers, Opretion.Print);
            if (isAuthorized != null) return  Ok(isAuthorized);
            var report = new WebReport();
            report = await _iRPT_Sales.ItemSalesForCustomersReport(parm, exportType, isArabic, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemsSalesForCustomers", exportType));
        }
        [HttpGet("getItemsNotSold")]
        public async Task<ResponseResult> getItemsNotSold([FromQuery] itemsNotSoldRequstDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.ItemNotSold, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.ItemsNotSold(parm);
        }
        [HttpGet("ItemsNotSoldReport")]
        public async Task<IActionResult> ItemsNotSoldReport([FromQuery] itemsNotSoldRequstDTO parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.ItemNotSold, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();



            report = await _iRPT_Sales.ItemsNotSoldReport(parm, exportType, isArabic, fileId);

            return Ok(_iPrintFileService.PrintFile(report, "ItemsNotSold", exportType));
            
        }
        [HttpGet("salesAndSalesReturnTransaction")]
        public async Task<ResponseResult> salesAndSalesReturnTransaction([FromQuery] salesAndSalesReturnTransactionRequstDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesAndReturnSalesTransaction, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.salesAndSalesReturnTransaction(parm);
        }
        [HttpGet("salesAndSalesReturnTransactionReport")]
        public async Task<IActionResult> salesAndSalesReturnTransactionReport([FromQuery] salesAndSalesReturnTransactionRequstDTO parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesAndReturnSalesTransaction, Opretion.Print);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();



            report = await _iRPT_Sales.salesAndSalesReturnTransactionReport(parm, exportType, isArabic, fileId );

            return Ok(_iPrintFileService.PrintFile(report, "SalesAndSalesReturnTransaction", exportType));
            

        }
        [HttpGet("itemsSoldMost")]
        public async Task<ResponseResult> itemsSoldMost([FromQuery] itemsSoldMostRequstDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.itemsSoldMost(parm);
        }
        [HttpGet("itemsSoldMostReport")]
        public async Task<IActionResult> itemsSoldMostReport([FromQuery] itemsSoldMostRequstDTO parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Print);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();



            report = await _iRPT_Sales.ItemsSoldMostReport(parm, exportType, isArabic, fileId);

            return Ok(_iPrintFileService.PrintFile(report, "ItemsSoldMost", exportType));
           // return Ok();
            //return await _iRPT_Sales.itemsSoldMost(parm);
        }
        [HttpGet("totalBranchTransaction")]
        public async Task<ResponseResult> totalBranchTransaction([FromQuery] totalBranchTransactionRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_Sales.totalBranchTransaction(parm);
        }
        [HttpGet("TotalBranchTransactionReport")]
        public async Task<IActionResult> TotalBranchTransactionReport([FromQuery] totalBranchTransactionRequestDTO parm,exportType exportType,bool isArabic,string? expandedTypeIds, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Print);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();
            report= await _iRPT_Sales.TotalBranchTransactionReport(parm, exportType, isArabic, expandedTypeIds, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "TotalBranchTransaction", exportType));

        }
        [HttpGet("SalesOfSalesMan")]
        public async Task<IActionResult> SalesOfSalesMan([FromQuery]SalesOfSalesManRequest Reuest)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesOfSalesMan, Opretion.Open);
            if (isAuthorized != null) return Ok(isAuthorized);
            //var profit = await _mediator.Send(new profitPagesRequest());
            //if (profit.Result == Enums.Result.InProgress)
            //{
            //    return Ok(new ResponseResult
            //    {
            //        Result = Enums.Result.InProgress,
            //        Note = "Profit is in Progress"
            //    });
            //}
            var res =await _mediator.Send(Reuest);
            return Ok(res);
        }
        [HttpGet("SalesOfSalesManReport")]
        public async Task<IActionResult> SalesOfSalesManReport([FromQuery] SalesOfSalesManRequest request,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesOfSalesMan, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _iRPT_Sales.SalesOfSalesManReport(request, exportType, isArabic,fileId);
            return Ok(_iPrintFileService.PrintFile(report, "SalesOfSalesManReport", exportType));

        }
        [HttpGet("ItemsProfit")]
        public async Task<IActionResult> ItemsProfit([FromQuery] ItemsProfitRequest Reuest)
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Open);
            //if (isAuthorized != null) return Ok(isAuthorized);
            var profit = await _mediator.Send(new profitPagesRequest());
            if (profit.Result == Enums.Result.InProgress)
            {
                return Ok(new ResponseResult
                {
                    Result = Enums.Result.InProgress,
                    Note = "Profit is in Progress"
                });
            }
            var res = await _mediator.Send(Reuest);
            return Ok(res);
        }
        [HttpGet("ItemsProfitReport")]
        public async Task<IActionResult> ItemsProfitReport([FromQuery] ItemsProfitRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsSoldMost, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _iRPT_Sales.ItemsProfitReport(request, exportType, isArabic, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemsProfitReport", exportType));

        }
        
        [HttpGet("GetDebtAgingForCustomresOrSuppliersReport")]
        public async Task<ResponseResult> GetDebtAgingForCustomresOrSuppliersReport([FromQuery] DebtAgingForCustomersOrSuppliersRequest request)
        {

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetTotalVatData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            DebtAgingForCustomersOrSuppliersRequest param = new DebtAgingForCustomersOrSuppliersRequest()
            {
                dateTo = request.dateTo,
                branches = request.branches,
                salesmen = request.salesmen,
                persons = request.persons,
                Department = request.Department,
                DontshowInvoicesLessThanOne = request.DontshowInvoicesLessThanOne,  
                pageSize = request.pageSize,
                pageNumber = request.pageNumber
                
            };
            var result = await _iRPT_Sales.GetDebtAgingForCustomresOrSuplier(param);
            return result;
        }

        [HttpGet("GetDebtAgingForCustomresOrSuppliersReportPrint")]
        public async Task<IActionResult> GetDebtAgingForCustomresOrSuppliersReportPrint([FromQuery] DebtAgingForCustomersOrSuppliersRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var screenId = (request.Department == 8 ? (int)SubFormsIds.DebtAgingForCustomers : (int)SubFormsIds.DebtAgingForSupplier);
            string fileName = (screenId == (int)SubFormsIds.DebtAgingForCustomers ? "DebtAgingForCustomers" : "DebtAgingForSuppliers");

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, screenId, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            DebtAgingForCustomersOrSuppliersRequest param = new DebtAgingForCustomersOrSuppliersRequest()
            {
                dateTo = request.dateTo,
                branches = request.branches,
                salesmen = request.salesmen,
                persons = request.persons,
                Department = request.Department,
                DontshowInvoicesLessThanOne = request.DontshowInvoicesLessThanOne,
            };

            WebReport report = new WebReport();
            report = await _iRPT_Sales.DebtAgingForCustomresOrSuplierReport(param, exportType, isArabic, fileId);
            return Ok(_iPrintFileService.PrintFile(report, fileName, exportType));
        }

        [HttpGet("DetailedInvoicesReport")]
        public async Task<ResponseResult> DetailedInvoicesReport([FromQuery] DetialedInvoicesReportRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailedInvoices, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            var result = await _iRPT_Sales.GetDetailedInvoicesReport(request,false);
            return result;
        }
        [HttpGet("DetailedInvoicesPrint")]
        public async Task<IActionResult> DetailedInvoicesPrint([FromQuery] DetialedInvoicesReportRequest parameter,exportType exportType, bool isArabic, int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailedInvoices, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            var report = await _iRPT_Sales.DetailedInvoicesPrint(parameter,exportType,isArabic,fileId);

            return Ok(_iPrintFileService.PrintFile(report, "DetailedInvoicesReport", exportType));

        }

    }
}
