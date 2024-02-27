using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction
{
    public interface IpurchasesTransactionService
    {
        Task<ResponseResult> PurchasesTransaction(purchasesTransactionRequest request,bool isPrint=false);
        Task<WebReport> PurchasesTransactionReport(purchasesTransactionRequest request, exportType exportType, bool isArabic, int fileId = 0);

    }
}
