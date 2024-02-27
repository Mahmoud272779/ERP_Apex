using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Reports.StoreReports.MainData;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.MainData;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class reportsOfMainDataController : ApiStoreControllerBase
    {
        private readonly iRPT_MainData _iRPT_MainData;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _iPrintFileService;


        public reportsOfMainDataController(iRPT_MainData iRPT_MainData,
                                            iAuthorizationService iAuthorizationService,
                                            IActionResultResponseHandler responseHandler,
                                            IprintFileService iPrintFileService) : base(responseHandler)
        {
            _iRPT_MainData = iRPT_MainData;
            _iAuthorizationService = iAuthorizationService;
            _iPrintFileService = iPrintFileService;
        }


        [HttpGet("ItemsPrices")]
        public async Task<ResponseResult> ItemsPrices([FromQuery] itemsPricesRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsPrices, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iRPT_MainData.ItemsPrices(parm);
        }
        [HttpGet("ItemsPricesReport")]
        public async Task<IActionResult> ItemsPricesReport([FromQuery] itemsPricesRequestDTO parm,exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsPrices, Opretion.Print);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();
            report = await _iRPT_MainData.ItemsPricesReport(parm, exportType, isArabic,fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemsPrices", exportType));

        }
        [HttpGet("ItemsPricesReportForIOSAndroid")]
        public async Task<IActionResult> ItemsPricesReportForIOSAndroid([FromQuery] itemsPricesRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.itemsPrices, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _iRPT_MainData.ItemsPricesReport(parm, exportType, isArabic,fileId);
            return Ok(_iPrintFileService.PrintFile(report, "ItemsPrices", exportType.ExportToPdf));

        }



    }
}
