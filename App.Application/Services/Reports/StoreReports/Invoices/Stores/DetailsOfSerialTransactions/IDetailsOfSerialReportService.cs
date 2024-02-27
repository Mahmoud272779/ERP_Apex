using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.DetailsOfSerialTransactions
{
    public interface IDetailsOfSerialReportService
    {
        Task<ResponseResult> DetailsOfSerialTransactions(DetailsOfSerialsRequest request,bool isPrint =false);
        Task<WebReport> DetailsOfSerialTransactionsReport(DetailsOfSerialsRequest request, exportType exportType, bool isArabic, int fileId = 0);

    }
}
