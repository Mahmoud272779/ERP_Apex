using App.Domain.Models.Request.Store.Reports.MainData;
using App.Domain.Models.Response.Store.Reports.MainData;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.MainData
{
    public interface iRPT_MainData
    {
        #region Items Prices 
        public Task<itemsPricesResponse> getItemsPrices(itemsPricesRequestDTO parm,bool isPrint = false);
        public Task<ResponseResult> ItemsPrices(itemsPricesRequestDTO parm);
        #endregion
        Task<WebReport> ItemsPricesReport(itemsPricesRequestDTO parm, exportType exportRype, bool isArabic, int fileId = 0);
    }
}
