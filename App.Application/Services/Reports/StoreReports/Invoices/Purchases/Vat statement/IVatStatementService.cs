using App.Application.Handlers.Invoices.Vat.GetTotalVatData;
using App.Domain.Enums;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases
{
    public interface IVatStatementService
    {
        Task<ResponseResult> GetVatStatmentTransaction(VatStatmentRequest reques,bool isPrint=false);
        Task<WebReport> GetVatStatmentTransactionReport(VatStatmentRequest request, exportType exportType, bool isArabic, int fileId = 0);
        
    }
}
