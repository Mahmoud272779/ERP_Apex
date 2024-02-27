using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers
{
    public class CostCenterRepoController : ApiGeneralLedgerControllerBase
    {
        private readonly ICostCenterReport costCenterReport;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;


        public CostCenterRepoController(ICostCenterReport costCenterReport, iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IprintFileService printFileService) :
            base(ResponseHandler)
        {
            this.costCenterReport = costCenterReport;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }
        [HttpGet("GetCostCenterReportData")]
        public async Task<IActionResult> GetTopLevelBudgetReport([FromQuery] CostCenterReportRequest request)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers,request.financialAccountId != null ? (int)SubFormsIds.costCenterForAccount : (int)SubFormsIds.CostCenterReport_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var account = await costCenterReport.GetCostCenterReport(request);

            return Ok(account);
        }

        [HttpGet("CostCenterReport")]
        public async Task<IActionResult> CostCenterReport([FromQuery] CostCenterReportRequest request, exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenterReport_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await costCenterReport.CostCenterPrint(request, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "CostCenterReport", exportType));
        }
    }
}
