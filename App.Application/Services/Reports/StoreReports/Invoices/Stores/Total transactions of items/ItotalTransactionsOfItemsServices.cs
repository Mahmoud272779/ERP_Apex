using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Total_transactions_of_items
{
    public interface ItotalTransactionsOfItemsServices
    {
        Task<ResponseResult> getTotalTransactionsOfItems(totalTransactionsOfItemsRequest request, bool isPrint=false);
        Task<WebReport> TotalTransactionsOfItemReport(totalTransactionsOfItemsRequest request, exportType exportType, bool isArabic, int fileId = 0);

    }
}
