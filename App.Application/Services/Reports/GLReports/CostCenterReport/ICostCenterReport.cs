using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface ICostCenterReport
    {
        Task<ResponseResult> GetCostCenterReport(CostCenterReportRequest Parameter, bool isPrint=false);
        Task<WebReport> CostCenterPrint(CostCenterReportRequest Parameter, exportType exportType, bool isArabic, int fileId = 0);
    }
}
