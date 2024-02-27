using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier;
using App.Application.Services.Reports.Invoices.Purchases.Supplier_Account;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using App.Application.Handlers.Reports.Purchases.PurchasesTransaction;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Application.Services.Printing.PrintFile;
using App.Application.Handlers.Invoices.Vat.GetTotalVatData;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class reportsOfPurchasesController : ApiStoreControllerBase
    {
        private readonly IItemPurchasesForSupplierService ItemPurchasesForSupplierService;
        private readonly IItemsPurchasesService ItemsPurchasesService;
        private readonly IItemsPurchasesForSupplierService ItemsPurchasesForSupplierService;
        private readonly IpurchasesTransactionService purchasesTransactionService;
        private readonly IPersonAccountService SupplierAccountService;
        private readonly IVatStatementService VatStatmentServices;
        private readonly ISuppliersAccountService SuppliersAccountService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IPersonHelperService _personHelperService;
        private readonly iRpt_PurchasesService _iRpt_PurchasesService;
        private readonly IprintFileService _printFileService;
        private readonly IMediator _mediator;

        private readonly iRPT_Sales _iRPT_Sales;
       private  readonly IprintFileService _iPrintFileService;

        public reportsOfPurchasesController(IItemPurchasesForSupplierService ItemPurchasesForSupplierService,
                                          IItemsPurchasesService ItemsPurchasesService,
                                          IItemsPurchasesForSupplierService ItemsPurchasesForSupplierService,
                                          IpurchasesTransactionService purchasesTransactionService,
                                          IPersonAccountService SupplierAccountService,
                                          ISuppliersAccountService SuppliersAccountService,
                                          iAuthorizationService iAuthorizationService,
                                          IPersonHelperService personHelperService,
                                          iRpt_PurchasesService iRpt_PurchasesService,
                                          IActionResultResponseHandler ResponseHandler,
                                          IVatStatementService VatStatmentServices,
                                          IprintFileService printFileService,
                                          IMediator mediator,
                                          iRPT_Sales iRPT_Sales,
                                          IprintFileService iPrintFileService) : base(ResponseHandler)
        {
            this.ItemPurchasesForSupplierService = ItemPurchasesForSupplierService;
            this.ItemsPurchasesService = ItemsPurchasesService;
            this.ItemsPurchasesForSupplierService = ItemsPurchasesForSupplierService;
            this.purchasesTransactionService = purchasesTransactionService;
            this.SupplierAccountService = SupplierAccountService;
            this.SuppliersAccountService = SuppliersAccountService;
            _iAuthorizationService = iAuthorizationService;
            _personHelperService = personHelperService;
            _iRpt_PurchasesService = iRpt_PurchasesService;
            this.VatStatmentServices = VatStatmentServices;
            _printFileService = printFileService;
            _mediator = mediator;
            _iRPT_Sales = iRPT_Sales;
            _iPrintFileService = iPrintFileService;
        }

        [HttpGet("GetItemPurchasesForSupplierData")]
        public async Task<ResponseResult> GetItemPurchasesForSupplierData([FromQuery] ItemPurchasesForSupplierRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.supplierItemsPurchased_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ItemPurchasesForSupplierService.GetItemPurchasesForSupplierData(request);
            return result;
        }

        [HttpGet("ItemPurchasesForSupplierReport")]
        public async Task<IActionResult> ItemPurchasesForSupplierDataReport([FromQuery] ItemPurchasesForSupplierRequest request,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.supplierItemsPurchased_Purchases, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await ItemPurchasesForSupplierService.ItemPurchasesForSupplierReport(request, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(report, "ItemPurchasesForSupplier", exportType));
        }


        [HttpGet("GetItemsPurchasesData")]
        public async Task<ResponseResult> GetItemsPurchasesData([FromQuery] ItemsPurchasesRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.ItemsPurchases_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ItemsPurchasesService.GetItemsPurchasesData(parm);
            return result;
        }
        [HttpGet("GetItemsPurchasesDataFromSupplier")]
        public async Task<ResponseResult> GetItemsPurchasesDataFromSupplier([FromQuery] ItemsPurchasesRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SupplierItemsPurchases_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ItemsPurchasesService.GetItemsPurchasesData(parm);
            return result;
        }

        //[HttpGet("ItemsPurchasesDataFromSupplierReport")]
        //public async Task<IActionResult> ItemsPurchasesDataFromSupplierReport([FromQuery] ItemsPurchasesRequest parm,exportType exportType,bool isArabic)
        //{
        //    var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SupplierItemsPurchases_Purchases, Opretion.Open);
        //    if (isAuthorized != null)
        //        return Ok(isAuthorized);
        //    WebReport report = new WebReport();

        //    report = await ItemsPurchasesService.ItemsPurchasesDataReport(parm, exportType, isArabic);


        //    return Ok(_printFileService.PrintFile(report, "ItemsPurchasesFromSupplierReport"));

        //}
        [HttpGet("ItemsPurchasesDataReport")]
        public async Task<IActionResult> ItemsPurchasesDataReport([FromQuery] ItemsPurchasesRequest param,exportType exportType, bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemPurchases_Purchases, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
             report = await ItemsPurchasesService.ItemsPurchasesDataReport(param, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "itemPurchases", exportType));


        }


        [HttpGet("GetItemsPurchasesForSupplierData")]
        public async Task<ResponseResult> GetItemsPurchasesForSupplierData([FromQuery] ItemsPurchasesForSupplierRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SupplierItemsPurchases_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ItemsPurchasesForSupplierService.GetItemsPurchasesForSupplierData(parm);
            return result;
        }
        [HttpGet("ItemsPurchasesForSupplierReport")]
        public async Task<IActionResult> ItemsPurchasesForSupplierReport([FromQuery] ItemsPurchasesForSupplierRequest parm, exportType exportType, bool isArabic, int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SupplierItemsPurchases_Purchases, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await ItemsPurchasesForSupplierService.ItemsPurchasesForSupplierReport(parm, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(report, "ItemsPurchasesForSupplier", exportType));
        }

        [HttpGet("PurchasesTransaction")]
        public async Task<ResponseResult> PurchasesTransaction([FromQuery] purchasesTransactionRequest Request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.purchasesTransaction_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
           
            var result = await purchasesTransactionService.PurchasesTransaction(Request);
            return result;
        }
        [HttpGet("PurchasesTransactionReport")]
        public async Task<IActionResult> PurchasesTransactionReport([FromQuery]purchasesTransactionRequest request, exportType exportType,bool isArabic,int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.purchasesTransaction_Purchases, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await purchasesTransactionService.PurchasesTransactionReport(request, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(report, "PurchasesTransaction", exportType));
        }


        [HttpGet("GetPersonAccountData")]
        public async Task<ResponseResult> GetPersonAccountData(int personId, string? branches, DateTime dateFrom, DateTime dateTo, bool PaidPurchase, int PageSize, int pageNumber)
        {

            int[] branchesIds = null;
            if (!string.IsNullOrEmpty(branches))
                branchesIds = Array.ConvertAll(branches.Split(','), s => int.Parse(s));

            SupplierAccountRequest request = new SupplierAccountRequest()
            {
                personId = personId,
                PaidPurchase = PaidPurchase,
                Branches = branchesIds,
                DateFrom = dateFrom,
                DateTo = dateTo,
                PageSize = PageSize,
                PageNumber = pageNumber
            };
            var result = await SupplierAccountService.GetPersonAccountData(request);
            return result;
        }
       
        [HttpGet("GetPersonAccountDataReport")]
        public async Task<IActionResult> GetPersonAccountDataReport(int personId, string? branches, DateTime dateFrom, DateTime dateTo, bool PaidPurchase, int PageSize, int pageNumber, exportType exportType,bool isArabic,int fileId)
        {
            var isCustomer = await _personHelperService.checkIsCustomer(personId);
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, isCustomer ? (int)SubFormsIds.CustomerStatement_Sales : (int)SubFormsIds.SupplierStatement_Purchases, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            int[] branchesIds = null;
            if (!string.IsNullOrEmpty(branches))
                branchesIds = Array.ConvertAll(branches.Split(','), s => int.Parse(s));
           

            SupplierAccountRequest request = new SupplierAccountRequest()
            {
                personId = personId,
                PaidPurchase = PaidPurchase,
                Branches = branchesIds,
                DateFrom = dateFrom,
                DateTo = dateTo,
                PageSize = PageSize,
                PageNumber = pageNumber
            };

            WebReport report = new WebReport();
            report= await SupplierAccountService.GetPersonAccountDataReport(request, exportType, isArabic,fileId);

            return Ok( _printFileService.PrintFile(report, "PersonsAccounts", exportType));


        }

        [HttpGet("GetVatStatmentData")]
        public async Task<ResponseResult> GetVatStatmentData([FromQuery] string branches, DateTime DateFrom, DateTime DateTo, int? InvoiceType, bool prevBalance, int PageSize, int pageNumber)
        {

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetVatStatmentData_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            VatStatmentRequest vatST = new VatStatmentRequest();
            vatST.dateTo = DateTo;
            vatST.dateFrom = DateFrom;
            vatST.branches = branches;
            vatST.InvoiceType = InvoiceType;
            vatST.prevBalance = prevBalance;
            vatST.PageSize = PageSize;
            vatST.PageNumber = pageNumber;

            return await VatStatmentServices.GetVatStatmentTransaction(vatST);
        }
        [HttpGet("GetVatStatmentDataReport")]
      //  public async Task<ReportsReponse> GetVatStatmentDataReport([FromQuery] string branches, DateTime DateFrom, DateTime DateTo, int? InvoiceType, bool prevBalance, int PageSize, int pageNumber, exportType exportType, bool isArabic)
        public async Task<IActionResult> GetVatStatmentDataReport([FromQuery] string branches, DateTime DateFrom, DateTime DateTo, int? InvoiceType, bool prevBalance, int PageSize, int pageNumber,exportType exportType,bool isArabic, int fileId)
        {
            
                var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetVatStatmentData_Purchases, Opretion.Print);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
                VatStatmentRequest vatST = new VatStatmentRequest();
                vatST.dateTo = DateTo;
                vatST.dateFrom = DateFrom;
                vatST.branches = branches;
                vatST.InvoiceType = InvoiceType;
                vatST.prevBalance = prevBalance;
                vatST.PageSize = PageSize;
                vatST.PageNumber = pageNumber;


                WebReport report = new WebReport();
                report = await VatStatmentServices.GetVatStatmentTransactionReport(vatST, exportType, isArabic,fileId);

                return Ok(_printFileService.PrintFile(report, "VatStatmentDataPurchases", exportType));

            

          //  return await VatStatmentServices.GetVatStatmentTransactionReport(vatST,exportType,isArabic);
        }


        [HttpGet("GetSuppliersAccountData")]
        public async Task<ResponseResult> GetSuppliersAccountData([FromQuery] SuppliersAccountRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetSuppliersAccountData_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            request.IsSupplier = true;
            var result = await SuppliersAccountService.GetSuppliersAccountData(request,false, false);
            return result;
        }
        [HttpGet("SuppliersCustomersBalancesReport")]
        public async Task<IActionResult> SuppliersCustomersBalancesReport([FromQuery] SuppliersAccountRequest request, exportType exportType,bool isArabic, int fileId )
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetSuppliersAccountData_Purchases, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);

            //SuppliersAccountRequest request = new SuppliersAccountRequest()
            //{
            //    zerosPrices = zerosPrices,
            //    Branches = branches,
            //    DateFrom = dateFrom,
            //    DateTo = dateTo,
            //    PageSize = PageSize,
            //    PageNumber = pageNumber
            //};
            request.IsSupplier = true;
            WebReport report = new WebReport();
            report = await SuppliersAccountService.SuppliersCustomersBalancesReport(request,false, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(report, "SuppliersBalances", exportType));
            //var result = await SuppliersAccountService.GetSuppliersAccountData(request);
            //return result;
        }
        [HttpGet("VatDetailedReport")]
        public async Task<ResponseResult> VatDetailedReport([FromQuery] VATDetailedReportRequest parm)
        {
           var isAuthorized = await  _iAuthorizationService.isAuthorized(0,(int)SubFormsIds.VatDetailedStatement, Opretion.Open);
           if (isAuthorized != null)
                return isAuthorized;
            var result = await _iRpt_PurchasesService.VatDetailedReport(parm);
            return result;
        }
        [HttpGet("GetVatDetailedReportPrint")]
       // public async Task<ReportsReponse> GetVatDetailedReportPrint([FromQuery] VATDetailedReportRequest parm, exportType exportType, bool isArabic)
        public async Task<IActionResult> GetVatDetailedReportPrint([FromQuery] VATDetailedReportRequest parm,exportType exportType, bool isArabic, int fileId = 0)
        {


            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.VatDetailedStatement, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            WebReport report = new WebReport();

            report = await _iRpt_PurchasesService.VatDetailedReportPrint(parm, exportType, isArabic,fileId);

            var data = _printFileService.PrintFile(report, "VatDetailedReport", exportType);
            return Ok(data);


        }
        [HttpGet("purchasesTransactionOfBranch")]
        public async Task<IActionResult> purchasesTransactionOfBranch([FromQuery] puchasesTransactionRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.purchasesTransactionOfBranch, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var data = await _mediator.Send(parm);
            return Ok(data);    


        }
        [HttpGet("purchasesTransactionOfBranchReport")]
        public async Task<IActionResult> purchasesTransactionOfBranchReport([FromQuery] puchasesTransactionRequest parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)SubFormsIds.purchasesTransactionOfBranch, (int)SubFormsIds.purchasesTransactionOfBranch, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
           
          
            WebReport report = new WebReport();



            report = await _iRPT_Sales.SalesTransactionReport(parm, (int)SubFormsIds.purchasesTransactionOfBranch, exportType, isArabic,fileId);

            return Ok(_iPrintFileService.PrintFile(report, "PurchaseTransActionOfBrach", exportType));


        }
        [HttpGet("GetTotalVatData")]
        public async Task<ResponseResult> GetTotalVatData([FromQuery] string branches , DateTime DateFrom, DateTime DateTo)
        {

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetTotalVatData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            VATTotalsReportRequest param = new VATTotalsReportRequest()
            {
                branchId = branches,
                dateFrom = DateFrom,
                dateTo = DateTo
            };
            var result = await _iRpt_PurchasesService.VatTotalsReport(param);
            return result;
        }
        [HttpGet("GetBranchesVatData")]
        public async Task<ResponseResult> GetBranchesVatData([FromQuery] string branches, DateTime DateFrom, DateTime DateTo, int invoiceType)
        {

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetTotalVatData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            VATTotalsReportRequest param = new VATTotalsReportRequest()
            {
                branchId = branches,
                dateFrom = DateFrom,
                dateTo = DateTo,
                InvoiceType = invoiceType
            };
            var result = await _iRpt_PurchasesService.GetBranchesVatReport(param);
            return result;
        }
        [HttpGet("TotalVatReport")]
        public async Task<IActionResult> TotalVatReport([FromQuery] VATTotalsReportRequest request, exportType exportType, bool isArabic,string? expandedTypeId, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.GetTotalVatData, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();

            report = await _iRpt_PurchasesService.TotalVatReportPrint(request, exportType, isArabic, expandedTypeId, fileId);
            return Ok(_iPrintFileService.PrintFile(report, "TotalVatReport", exportType));


            //return Ok();

        }
        //[HttpGet("GetDebtAgingForSuppliers")]
        //public async Task<ResponseResult> GetDebtAgingForSuppliers([FromQuery] string branches , int SupplierCode , string SuppliersNames)

    }
}
