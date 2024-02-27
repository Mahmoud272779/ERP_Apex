using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.GeneralLedger.ReportsController
{
    public class BudgetController : ApiGeneralLedgerControllerBase
    {
        private readonly IBudgetReportService budgetService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;


        public BudgetController(IBudgetReportService budgetService, iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IprintFileService printFileService) :
            base(ResponseHandler)
        {
            this.budgetService = budgetService;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }
        [HttpGet("GetTopLevelBudgetReport")]
        public async Task<IActionResult> GetTopLevelBudgetReport(DateTime From, DateTime To)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.PublicBudget_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();

            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.Balance;
            var account = await budgetService.getTopLevelBudget(parameters);

            return Ok(account);
        }
        [HttpGet("PublicBudgetReport")]

        public async Task<IActionResult> PublicBudgetReport(DateTime From, DateTime To,string? ids,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.PublicBudget_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();

            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.Balance;
            WebReport report = new WebReport();

            report = await budgetService.PublicBudgetReport(parameters,ids,exportType,isArabic,fileId);
            return Ok(_printFileService.PrintFile(webReport: report, "PublicBudget", exportType));
        }

        [HttpGet("GetAllDataBudgetById")]
        public async Task<IActionResult> GetAllDataBudgetById(int Id, DateTime From, DateTime To)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.PublicBudget_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();

            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.Balance;
            var account = await budgetService.getAllDataBudgetById(parameters, Id);

            return Ok(account);
        }
    }
}
