using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices
{
    public interface iSafeOrBankReportService
    {
        #region Account Statement
        public Task<bankAndSafesResponse> getBanksOrSafeAccountStatement(safesRequestDTO parm,bool isPrint = false);
        public Task<ResponseResult> BanksOrSafeAccountStatement(safesRequestDTO parm);
        #endregion
        #region Expenses and receipts
        public Task<ExpensesAndReceiptsResponse> getExpensesAndReceipts(safesRequestDTO parm, bool isCash, bool isPrint = false);
        public Task<ResponseResult> ExpensesAndReceipts(safesRequestDTO parm,bool isCash);
        #endregion
        #region Receipts and disbursements
        public Task<PaymentsAndDisbursementsResponse> getPaymentsAndDisbursements(PaymentsAndDisbursementsRequestDTO parm, bool isPrint = false);
        public Task<ResponseResult> PaymentsAndDisbursements(PaymentsAndDisbursementsRequestDTO parm);
        #endregion
        
        Task<WebReport> BanksOrSafeAccountStatementReport(safesRequestDTO parm, exportType exportType, bool isArabic,int fileId=0);
        Task<WebReport> PaymentsAndDisbursementsReport(PaymentsAndDisbursementsRequestDTO param, exportType exportType, bool isArabic,int fileId=0);
        Task<WebReport> SafeBankExpensesReceiptsReport(safesRequestDTO request, bool isCash, exportType exportType, bool isArabic,int fileId=0);



    }
}
