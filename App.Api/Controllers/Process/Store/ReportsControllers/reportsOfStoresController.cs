using App.Api.Controllers.BaseController;
using App.Application.Handlers.Profit;
using App.Application.Handlers.Profit.profitPages;
using App.Application.Handlers.Reports.Store.Store.DetailedTransactoinsOfItem;
using App.Application.Handlers.Reports.Store.Store.getItemBalanceInStores;
using App.Application.Handlers.Reports.Store.Store.ItemsBalanceInStores;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Reports.Items_Prices;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.DetailsOfSerialTransactions;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Total_transactions_of_items;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class reportsOfStoresController : ApiStoreControllerBase
    {
        private readonly IDetailedTransOfItemService DetailedTransOfItemService;
        private readonly IitemBalanceInStoresService itemBalanceInStoresService;
        private readonly IRpt_Store _rpt_Store;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly ItotalTransactionsOfItemsServices totalTransactionsOfItemsServices;
        private readonly IDetailsOfSerialReportService _DetailsOfSerialReportService;
        private readonly IprintFileService _printFileService;
        private readonly IMediator _mediatR;

        public reportsOfStoresController(IDetailedTransOfItemService DetailedTransOfItemService,
                                          IitemBalanceInStoresService itemBalanceInStoresService,
                                          IRpt_Store rpt_Store,
                                          iAuthorizationService iAuthorizationService,
                                          ItotalTransactionsOfItemsServices totalTransactionsOfItemsServices,
                                          IDetailsOfSerialReportService DetailsOfSerialReportService,
                  IActionResultResponseHandler responseHandler,
                  IprintFileService printFileService,
                  IMediator mediatR) : base(responseHandler)
        {
            this.DetailedTransOfItemService = DetailedTransOfItemService;
            this.itemBalanceInStoresService = itemBalanceInStoresService;
            _rpt_Store = rpt_Store;
            _iAuthorizationService = iAuthorizationService;
            this.totalTransactionsOfItemsServices = totalTransactionsOfItemsServices;
            _DetailsOfSerialReportService = DetailsOfSerialReportService;
            _printFileService = printFileService;
            _mediatR = mediatR;
        }


        [HttpGet("DetailedTransactoinsOfItem")]
        public async Task<ResponseResult> DetailedTransactoinsOfItem([FromQuery] DetailedTransactoinsOfItemRequest /*DetailedMovementOfanItemReqest*/ parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailedMovementOfAnItem_Repository, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            //var res = await _rpt_Store.DetailedMovementOfanItem(parm);
            parm.isPrint = false;   
            var res = await _mediatR.Send(parm);
            return res;
        }
        [HttpGet("DetailedTransactoinsOfItemReport")]
        public async Task<IActionResult> DetailedTransactoinsOfItemReport([FromQuery] DetailedMovementOfanItemReqest parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailedMovementOfAnItem_Repository, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);

            WebReport report = new WebReport();
            report = await _rpt_Store.DetailedMovementOfanItemReport(parm, exportType, isArabic,fileId);


            return Ok(_printFileService.PrintFile(report, SavedFileName: "DetailedMovementOfAnItem", exportType));


        }

        [HttpGet("getItemBalanceInStores")]
        public async Task<ResponseResult> getItemBalanceInStores(getItemBalanceInStoresRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.getItemBalanceInStores_Repository, Domain.Enums.Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var res = await itemBalanceInStoresService.getItemBalanceInStores(itemId);
            parm.isPrint = false;
            var res = await _mediatR.Send(parm);
            return res;
        }
        [HttpGet("ItemBalanceInStoresReport")]
        public async Task<IActionResult> ItemBalanceInStoresReport(int itemId, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.getItemBalanceInStores_Repository, Domain.Enums.Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);


            WebReport report = new WebReport();
            report = await itemBalanceInStoresService.ItemBalanceInStoresReport(itemId, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "ItemBalanceInStores", exportType));
        }


        [HttpGet("getTotalTransactionsOfItems")]
        public async Task<ResponseResult> getTotalTransactionsOfItems([FromQuery] totalTransactionsOfItemsRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.getTotalTransactionsOfItems_Repository, Domain.Enums.Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            var res = await totalTransactionsOfItemsServices.getTotalTransactionsOfItems(request);
            return res;
        }
        [HttpGet("TotalTransactionsOfItemReport")]
        public async Task<IActionResult> TotalTransactionsOfItemReport([FromQuery] totalTransactionsOfItemsRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.getTotalTransactionsOfItems_Repository, Domain.Enums.Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await totalTransactionsOfItemsServices.TotalTransactionsOfItemReport(request, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "TotalTransactionsOfItem", exportType));

        }
        [HttpGet("InventoryValuation")]
        public async Task<ResponseResult> InventoryValuation([FromQuery] InventoryValuationRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(mainFormCode: (int)MainFormsIds.Repository, (int)SubFormsIds.InventoryValuation_Repository, Domain.Enums.Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            if(request.inventoryValuationType == InventoryValuationType.CostPrice)
            {
                var profit = await _mediatR.Send(new profitPagesRequest());
                if (profit.Result == Enums.Result.InProgress)
                {
                    return new ResponseResult
                    {
                        Result = Enums.Result.InProgress,
                        Note = "Profit is in Progress"
                    };
                }
            }
            var res = await _rpt_Store.InventoryValuation(request);
            return res;
        }
        [HttpGet("InventoryValuationReport")]
        public async Task<IActionResult> InventoryValuationReport([FromQuery] InventoryValuationRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(mainFormCode: (int)MainFormsIds.Repository, (int)SubFormsIds.InventoryValuation_Repository, Domain.Enums.Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _rpt_Store.InventoryValuationReport(request, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "InventoryValuation", exportType));

        }
        [HttpGet("GetDetailsOfSerialTransactions")]
        public async Task<ResponseResult> GetDetailsOfSerialTransactions([FromQuery] DetailsOfSerialsRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailsOfSerialTransactions, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            if (string.IsNullOrEmpty(request.serial))
                return new ResponseResult()
                {
                    Note = "Serial is Required",
                    Result = Result.Failed
                };

            var res = await _DetailsOfSerialReportService.DetailsOfSerialTransactions(request);
            return res;
        }
        [HttpGet("DetailsOfSerialTransactionsReport")]
        public async Task<IActionResult> DetailsOfSerialTransactionsReport([FromQuery] DetailsOfSerialsRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DetailsOfSerialTransactions, Opretion.Open);
            if (isAuthorized != null) return Ok(isAuthorized);
            if (string.IsNullOrEmpty(request.serial))
                return Ok(new ResponseResult()
                {
                    Note = "Serial is Required",
                    Result = Result.Failed
                });
            WebReport report = new WebReport();
            report = await _DetailsOfSerialReportService.DetailsOfSerialTransactionsReport(request, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "DetailsOfSerialTransactions", exportType));


        }
        [HttpGet("TotalTransactionOfItems")]
        public async Task<ResponseResult> TotalTransactionOfItems([FromQuery] TotalTransactionOfItemsRequestDTO request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.getTotalTransactionsOfItems_Repository, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            var res = await _rpt_Store.TotalTransactionOfItems(request);
            return res;
        }

        [HttpGet("GetDetailsOfExpiredItemsForPrint")]
        public async Task<IActionResult> GetDetailsOfExpiredItemsForPrint([FromQuery] ExpiredItemsReportForPrintRequestDTO request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetDetailsOfExpiredItems, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            if (request.storeId == 0 || request.NumberOfDays == 0)
                return null;

            ExpiredItemsReportRequestDTO parm = new ExpiredItemsReportRequestDTO()
            {
                NumberOfDays = request.NumberOfDays,
                storeId = request.storeId,
            };

            var res = await _rpt_Store.GetDetailsOfExpiredItems(parm, true);
            return Ok(res);
        }
        [HttpGet("DetailsOfExpiredItemsReport")]
        public async Task<IActionResult> DetailsOfExpiredItemsReport([FromQuery] ExpiredItemsReportForPrintRequestDTO request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetDetailsOfExpiredItems, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            if (request.storeId == 0)
                return Ok( new ResponseResult() { Note = "Store Id is required", Result = Result.Failed });

            if (request.NumberOfDays == 0)
                return Ok( new ResponseResult() { Note = "Number Of Days is required", Result = Result.Failed });


            ExpiredItemsReportRequestDTO parm = new ExpiredItemsReportRequestDTO()
            {
                NumberOfDays = request.NumberOfDays,
                storeId = request.storeId,
            };
            WebReport report = new WebReport();
            report = await _rpt_Store.DetailsOfExpiredItemsReport(parm, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "ExpiredItems", exportType));
        }

        [HttpGet("GetDetailsOfExpiredItems")]
        public async Task<ResponseResult> GetDetailsOfExpiredItems([FromQuery] ExpiredItemsReportRequestDTO request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetDetailsOfExpiredItems, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            if (request.storeId == 0)
                return new ResponseResult() { Note = "Store Id is required", Result = Result.Failed };

            if (request.NumberOfDays == 0)
                return new ResponseResult() { Note = "Number Of Days is required", Result = Result.Failed };

            var res = await _rpt_Store.DetailsOfExpiredItems(request);
            return res;
        }
        [HttpGet("DemandLimit")]
        public async Task<ResponseResult> DemandLimit([FromQuery] DemandLimitRequestDTO request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DemandLimit, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            var res = await _rpt_Store.DemandLimit(request);
            return res;
        }
        [HttpGet("DemandLimitReport")]
        public async Task<IActionResult> DemandLimitReport([FromQuery] DemandLimitRequestDTO request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.DemandLimit, Opretion.Open);
            if (isAuthorized != null) return Ok(isAuthorized);

            WebReport report = new WebReport();
            report = await _rpt_Store.DemandLimitReport(request, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "DemandLimit", exportType));
        }
        [HttpGet("ItemsBalanceInStores")]
        public async Task<ResponseResult> ItemsBalanceInStores([FromQuery] _ItemsBalanceInStoresRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.TotalItemBalancesInStore_Repository, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            //var res = await itemBalanceInStoresService.ItemsBalanceInStores(request);
            var res = await _mediatR.Send(request);
            return res;
        }
        [HttpGet("ItemsBalanceInStoresReport")]
        public async Task<IActionResult> ItemsBalanceInStoresReport([FromQuery] itemsBalanacesInStoreRequestDTO request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.TotalItemBalancesInStore_Repository, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await itemBalanceInStoresService.ItemsBalanceInStoresReport(request, exportType, isArabic ,fileId);
            return Ok(_printFileService.PrintFile(report, "ItemsBalanceInStoresReport", exportType));

        }
        [HttpGet("ReviewWarehouseTransfers")]
        public async Task<ResponseResult> ReviewWarehouseTransfers([FromQuery] ReviewWarehouseTransfersRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.ReviewWarehouseTransfersReport, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            var res = await _rpt_Store.ReviewWarehouseTransfers(parm);
            return res;
        }
        [HttpGet("ReviewWarehouseTransfersReport")]
        public async Task<IActionResult> ReviewWarehouseTransfersReport([FromQuery] ReviewWarehouseTransfersRequest parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.ReviewWarehouseTransfersReport, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _rpt_Store.ReviewWarehouseTransfersReport(parm, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "ReviewWarehouseTransfers", exportType));
        }
        [HttpGet("DetailedTransferReport")]
        public async Task<ResponseResult> DetailedTransferReport([FromQuery] DetailedTransferReportRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.DetailedTransferReport, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            var res = await _rpt_Store.DetailedTransferReport(parm);
            return res;
        }
        [HttpGet("DetailedTransferPrint")]
        public async Task<IActionResult> DetailedTransferPrint([FromQuery] DetailedTransferReportRequest parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.DetailedTransferReport, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);


            WebReport report = new WebReport();
            report = await _rpt_Store.DetailedTransferPrint(parm, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "DetailedTransfers", exportType));
        }
    }
}
