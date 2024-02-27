using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.BalanceReview;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.GeneralLedger.ReportsController
{
    public class IncomeListController : ApiGeneralLedgerControllerBase
    {
        private readonly IIncomeListBusiness incomeListBusiness;
        private readonly iAuthorizationService _iAuthorizationService;
        public readonly IIncomeListAndBudget _iIncomingList;
        private readonly IprintFileService _printFileService;


        public IncomeListController(IIncomeListBusiness IncomeListBusiness, iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IIncomeListAndBudget iIncomingList, IprintFileService printFileService) :
            base(ResponseHandler)
        {
            incomeListBusiness = IncomeListBusiness;
            _iAuthorizationService = iAuthorizationService;
            _iIncomingList = iIncomingList;
            _printFileService = printFileService;
        }
        [HttpGet("GetTopLevelIncomeListReport")]
        public async Task<IActionResult> GetIncomeListReport(DateTime From, DateTime To)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.IncomeList_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();
            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.IncomingList;
            var account = await incomeListBusiness.getTopLevelIncomingList(parameters);

            return Ok(account);
        }
        [HttpGet("IncomeListReport")]
        public async Task<IActionResult> IncomeListReport(DateTime From, DateTime To,string? ids,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.IncomeList_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();
            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.IncomingList;
            WebReport report = new WebReport();

            report = await _iIncomingList.IncomingListReport(parameters, ids, exportType, isArabic,fileId);

            return Ok(_printFileService.PrintFile(webReport: report, "IncomeList", exportType));
        }

        [HttpGet("GetIncomeListReport")]
        public async Task<IActionResult> GetIncomeListById(int Id, DateTime From, DateTime To)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.IncomeList_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            IncomeListSearchParameter parameters = new IncomeListSearchParameter();
            parameters.From = From;
            parameters.To = To;
            parameters.finalAccount = (int)finalAccount.IncomingList;
            var account = await incomeListBusiness.getAllDataIncomeinListById(parameters, Id);

            return Ok(account);
        }
    }
}
