using App.Domain.Enums;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores
{
    public interface IitemBalanceInStoresService
    {
        Task<ResponseResult> getItemBalanceInStores(int itemId);
        #region ItemsBalanceInStores
        Task<itemBalanceInStoreResponse> getItemsBalanceInStores(itemsBalanacesInStoreRequestDTO parm,bool isPrint = false);
        Task<ResponseResult> ItemsBalanceInStores(itemsBalanacesInStoreRequestDTO parm);
        #endregion
        Task<WebReport> ItemsBalanceInStoresReport(itemsBalanacesInStoreRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> ItemBalanceInStoresReport(int itemId, exportType exportType, bool isArabic, int fileId = 0);

    }
}
