using App.Domain.Models.Request.Store.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.salesProfit
{
    public interface IRPT_SalesProfit
    {
        public  Task<ResponseResult> GetSalesProfit(RPT_SalesProfitRequest Parameter,bool isPrint=false);
        public  Task<WebReport> SalesProfitReport(RPT_SalesProfitRequest Parameter, exportType exportType, bool isArabic,int fileId=0);

    }
}
