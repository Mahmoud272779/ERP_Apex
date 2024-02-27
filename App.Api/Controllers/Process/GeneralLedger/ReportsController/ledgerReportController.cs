using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.GLServices.ledger_Report;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Office2010.Word;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.GeneralLedger.ReportsController
{
    public class ledgerReportController : ApiGeneralLedgerControllerBase
    {
        private readonly ILedgerReportService ledgerReportService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _iPrintFileService;


        public ledgerReportController(ILedgerReportService ledgerReportService,iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IprintFileService iPrintFileService) :
            base(ResponseHandler)
        {
            this.ledgerReportService = ledgerReportService;
            _iAuthorizationService = iAuthorizationService;
            _iPrintFileService = iPrintFileService;
        }
        [HttpGet("GetledgerReportLedgerReport")]
        public async Task<IActionResult> GetledgerReportLedgerReport(int PageNumber, int PageSize,
             int AccountID, DateTime From,  DateTime To )
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.LedgerReport_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            PageParameterLedgerReport paramters = new PageParameterLedgerReport()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                AccountId = AccountID,
                From = From,
                To = To
            };

         
       
            var res = await ledgerReportService.GetLedgerData(paramters);
            if (res.Result == Result.Success)
                return Ok(res);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, res);

        }
        [HttpGet("GetledgerReportAccountStatementDetail")]
        public async Task<IActionResult> GetledgerReportAccountStatementDetail(int PageNumber, int PageSize,
             int AccountID, DateTime From,  DateTime To ,int documentId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.LedgerReport_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            PageParameterLedgerReport paramters = new PageParameterLedgerReport()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                AccountId = AccountID,
                From = From,
                To = To,
                DecumentId= documentId

            };

         
       
            var res = await ledgerReportService.GetLedgerData(paramters);
            if (res.Result == Result.Success)
                return Ok(res);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, res);

        }
        [HttpGet("AccountStatementDetailReport")]
        public async Task<IActionResult> AccountStatementDetailReport(int PageNumber, int PageSize,
            int AccountID, DateTime From, DateTime To, SubFormsIds accountType, exportType exportType, bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)SubFormsIds.AccountStatementDetail_GL, (int)SubFormsIds.LedgerReport_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            string reportName = "";
            PageParameterLedgerReport paramters = new PageParameterLedgerReport()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                AccountId = AccountID,
                From = From,
                To = To,
                DecumentId=(int)accountType
            };

            WebReport report = new WebReport();

            report = await ledgerReportService.ledgerReport(paramters,accountType, exportType, isArabic,fileId);
            if ((int)accountType == 22)
            {
                reportName = "LedgerData";
            }
            else if ((int)accountType == 24)
            {
                reportName = "AccountStatementDetails";

            }

            return Ok(_iPrintFileService.PrintFile(report, reportName, exportType));

            //if (res.Result == Result.Success)
            //    return Ok(res);
            //else
            //    return StatusCode(StatusCodes.Status422UnprocessableEntity, res);

        }
    }
}
