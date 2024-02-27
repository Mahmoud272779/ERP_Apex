using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.BalanceReview;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Api.Controllers.Process.GeneralLedger.ReportsController
{
    public class BalanceReviewController : ApiGeneralLedgerControllerBase
    {
        private readonly IBalanceReviewBusiness balanceReviewBusiness;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;


        public BalanceReviewController(IBalanceReviewBusiness BalanceReviewReportBusiness, iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IprintFileService printFileService) :
            base(ResponseHandler)
        {
            balanceReviewBusiness = BalanceReviewReportBusiness;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }
        [HttpGet("getTopLevelDataBalance")]

        public async Task<IActionResult> getTopLevelDataBalance(int typeId , int FinantialAccountId,  DateTime From, DateTime To)
        {
            ResponseResult isAuth = await CheckAuthByType(typeId);
          if (isAuth != null)
                return Ok(isAuth);

            BalanceReviewSearchParameter parameters = new BalanceReviewSearchParameter();
            parameters.From = From;
            parameters.To = To;
            parameters.FinancialAcountId=FinantialAccountId;
            var account = await balanceReviewBusiness.getTopLevelDataBalance(parameters);
            //var result = ResponseHandler.GetResult(account);
            return Ok(account);
        }

        private async Task<ResponseResult> CheckAuthByType(int typeId)
        {
            ResponseResult isAuthorized = new ResponseResult();
            if (typeId == (int)BalanceReviewReportScreens.DetailedTrialBalance)
                isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.DetailedTrialBalance_GL, Opretion.Open);
            if (typeId == (int)BalanceReviewReportScreens.TotalAccountBalance)
                isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.TotalAccountBalance, Opretion.Open);
            if (typeId == (int)BalanceReviewReportScreens.BalanceReviewFunds)
                isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.BalanceReviewFunds, Opretion.Open);

            return isAuthorized;
        }



        [HttpGet("DetailedBalanceReviewReport")]

        public async Task<IActionResult> DetailedBalanceReviewReport(int typeId, int FinantialAccountId, DateTime From, DateTime To,string? ids,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.DetailedTrialBalance_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            BalanceReviewSearchParameter parameters = new BalanceReviewSearchParameter();
            parameters.From = From;
            parameters.To = To;
            parameters.TypeId = typeId;
            parameters.FinancialAcountId= FinantialAccountId;
            WebReport report = new WebReport();

            report = await balanceReviewBusiness.DetailedBalanceReviewReport(parameters,ids, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(report, "DetailedBalanceReview", exportType));
            
           
        }

        [HttpGet("getAllDataBalanceById")]
        public async Task<IActionResult> getAllDataBalanceById(int typeId ,int FinancialAcountId, DateTime From, DateTime To)
        {
            ResponseResult isAuth = await CheckAuthByType(typeId);
            if (isAuth != null)
                return Ok(isAuth);
            BalanceReviewSearchParameter parameters = new BalanceReviewSearchParameter();

            parameters.FinancialAcountId = FinancialAcountId;
            parameters.From = From;
            parameters.To = To;
            var account = await balanceReviewBusiness.getAllDataBalanceById(parameters, FinancialAcountId);
            if (account.Result == Domain.Enums.Enums.Result.RequiredData)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, account);
            return Ok(account);
        }
    }
}
