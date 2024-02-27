using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Reports.Items_Prices;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.DetailsOfSerialTransactions;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Total_transactions_of_items;
using App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class reportsOfSafesAndBanksController : ApiStoreControllerBase
    {
        private readonly iSafeOrBankReportService _iSafeOrBankReportService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _iPrintFileService;


        public reportsOfSafesAndBanksController(iSafeOrBankReportService iSafeOrBankReportService,
                  iAuthorizationService authorizationService,
                  IActionResultResponseHandler responseHandler, IprintFileService iPrintFileService) : base(responseHandler)
        {
            _iSafeOrBankReportService = iSafeOrBankReportService;
            _iAuthorizationService = authorizationService;
            _iPrintFileService = iPrintFileService;
        }

        
        [HttpGet("BanksOrSafeAccountStatement")]
        public async Task<ResponseResult> BanksOrSafeAccountStatement([FromQuery] safesRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0,parm.isSafe? (int)SubFormsIds.SafeAccountStatement : (int)SubFormsIds.BankAccountStatement, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iSafeOrBankReportService.BanksOrSafeAccountStatement(parm);
        }
        [HttpGet("BanksOrSafeAccountStatementReport")]
        public async Task<IActionResult> BanksOrSafeAccountStatementReport([FromQuery] safesRequestDTO param,exportType exportType, bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, param.isSafe ? (int)SubFormsIds.SafeAccountStatement : (int)SubFormsIds.BankAccountStatement, Opretion.Print);
            if (isAuthorized != null) return  Ok(isAuthorized);

            WebReport report = new WebReport();
            report= await _iSafeOrBankReportService.BanksOrSafeAccountStatementReport(param,exportType,isArabic,fileId);
            string reportName;
            if (param.isSafe)
            {
                reportName = "SafeAccountStatement";
            }
            else
            {
                reportName = "BankAccountStatement";

            }
            return Ok(_iPrintFileService.PrintFile(report, reportName, exportType));


        }
        [HttpGet("CashExpensesAndReceipts")]
        public async Task<ResponseResult> CashExpensesAndReceipts([FromQuery] safesRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, parm.isSafe ? (int)SubFormsIds.SafeReceipts : (int)SubFormsIds.BankExpenses, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iSafeOrBankReportService.ExpensesAndReceipts(parm,true);
        }
        [HttpGet("SafeBankExpensesReceiptsReport")]
        public async Task<IActionResult> SafeBankExpensesReceiptsReport([FromQuery] safesRequestDTO parm,bool isCash,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, parm.isSafe ? (int)SubFormsIds.SafeReceipts : (int)SubFormsIds.BankExpenses, Opretion.Open);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _iSafeOrBankReportService.SafeBankExpensesReceiptsReport(parm, true, exportType, isArabic,fileId);
            string reportName;
           
                if (parm.isSafe)
                {
                    reportName = "SafeExpenses";
                }
                else
                {
                    reportName = "BankExpenses";

                }
            //}
            //else
            //{
            //    if (parm.isSafe)
            //    {
            //        reportName = "SafeReceipts";
            //    }
            //    else
            //    {
            //        reportName = "BankReceipts";

            //    }
            //}
            
            return Ok(_iPrintFileService.PrintFile(report, reportName, exportType));

        }
        [HttpGet("PaymentExpensesAndReceipts")]
        public async Task<ResponseResult> PaymentExpensesAndReceipts([FromQuery] safesRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, parm.isSafe ? (int)SubFormsIds.SafeExpenses : (int)SubFormsIds.BankReceipts, Opretion.Print);
            if (isAuthorized != null) return isAuthorized;
            return await _iSafeOrBankReportService.ExpensesAndReceipts(parm,false);
        }
        [HttpGet("PaymentExpensesAndReceiptsReport")]
        public async Task<IActionResult> PaymentExpensesAndReceiptsReport([FromQuery] safesRequestDTO parm,exportType exportType,bool isArabic, int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, parm.isSafe ? (int)SubFormsIds.SafeExpenses : (int)SubFormsIds.BankReceipts, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);
            WebReport report = new WebReport();
            report = await _iSafeOrBankReportService.SafeBankExpensesReceiptsReport(parm, false, exportType, isArabic,fileId);
            
            return Ok(_iPrintFileService.PrintFile(report, "PaymentExpensesAndReceipts", exportType));

        }
        [HttpGet("PaymentsAndDisbursements")]
        public async Task<ResponseResult> PaymentsAndDisbursements([FromQuery] PaymentsAndDisbursementsRequestDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0,(int)SubFormsIds.PaymentsAndDisbursements, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;
            return await _iSafeOrBankReportService.PaymentsAndDisbursements(parm);
        }
        [HttpGet("PaymentsAndDisbursementsReport")]
        public async Task<IActionResult> PaymentsAndDisbursementsReport([FromQuery] PaymentsAndDisbursementsRequestDTO param,exportType exportType, bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.PaymentsAndDisbursements, Opretion.Open);
            if (isAuthorized != null) return Ok( isAuthorized);
            WebReport report = new WebReport();
           report= await  _iSafeOrBankReportService.PaymentsAndDisbursementsReport(param,exportType,isArabic,fileId);
            return Ok(_iPrintFileService.PrintFile(report, "PaymentsAndDisbursements", exportType));

        }
    }
}
