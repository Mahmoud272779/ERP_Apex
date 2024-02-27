using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account
{
    public interface ISuppliersAccountService
    {
        Task<ResponseResult> GetSuppliersAccountData(SuppliersAccountRequest request, bool isSales, bool isPrint=false);
        Task<WebReport> SuppliersCustomersBalancesReport(SuppliersAccountRequest request, bool isSales, exportType exportType, bool isArabic, int fileId = 0);

    }
}
