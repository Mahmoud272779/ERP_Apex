using App.Api.Controllers.BaseController;
using App.Application.Handlers.Profit;
using App.Application.Handlers.Profit.profitPages;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Reports.StoreReports.salesProfit;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Sales;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers.SalesProfit
{
    public class RPT_SalesProfitController : ApiStoreControllerBase
    {
        private readonly IRPT_SalesProfit salesProfit;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _iPrintFileService;
        private readonly IMediator _mediator;
        public RPT_SalesProfitController(IRPT_SalesProfit salesProfit, IActionResultResponseHandler ResponseHandler, iAuthorizationService iiAuthorizationService, IprintFileService iPrintFileService, IMediator mediator) : base(ResponseHandler)
        {
            this.salesProfit = salesProfit;
            _iAuthorizationService = iiAuthorizationService;
            _iPrintFileService = iPrintFileService;
            _mediator = mediator;
        }
        [HttpGet ("GetSalesProfitData")]
        public async Task<ResponseResult> GetSalesProfitData([FromQuery] RPT_SalesProfitRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesBranchProfit, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            var profit = await _mediator.Send(new profitPagesRequest());
            if (profit.Result == Enums.Result.InProgress)
            {
                return new ResponseResult
                {
                    Result = Enums.Result.InProgress,
                    Note = "Profit is in Progress"
                };
            }
            return await salesProfit.GetSalesProfit(parameter);
        }

        [HttpGet("SalesProfitReport")]
        public async Task<IActionResult> SalesProfitReport([FromQuery] RPT_SalesProfitRequest parameter,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesBranchProfit, Opretion.Open);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();



            report = await salesProfit.SalesProfitReport(parameter, exportType, isArabic,fileId);

            return Ok(_iPrintFileService.PrintFile(report, "SalesProfitOfBranch", exportType));
        }
    }
}
