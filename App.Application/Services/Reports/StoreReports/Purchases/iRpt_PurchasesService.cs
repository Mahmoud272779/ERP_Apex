using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports
{
    public interface iRpt_PurchasesService
    {
        public Task<ResponseResult> VatDetailedReport(VATDetailedReportRequest param,bool isPrint = false);
        //public Task<ResponseResult> VatDetailedReport(VATDetailedReportRequest param);
        Task<WebReport> VatDetailedReportPrint(VATDetailedReportRequest request, exportType exportType, bool isArabic, int fileId = 0);
        Task<ResponseResult> VatTotalsReport(VATTotalsReportRequest param, bool isPrint = false);
        Task<ResponseResult> GetBranchesVatReport(VATTotalsReportRequest param, bool isPrint = false);
        Task<WebReport> TotalVatReportPrint(VATTotalsReportRequest request, exportType exportType, bool isArabic, string expandedTypeId, int fileId = 0);

    }
}
