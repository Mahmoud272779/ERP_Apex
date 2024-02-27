using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GLServices.ledger_Report
{
    public interface ILedgerReportService
    {
        Task<ResponseResult> GetLedgerData(PageParameterLedgerReport paramters,bool isPrint=false);
        Task<WebReport> ledgerReport(PageParameterLedgerReport paramters, SubFormsIds accountType, exportType exportType, bool isarabic,int fileId=0);
    }
}
