using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases
{
    public interface IItemsPurchasesService
    {
        Task<ResponseResult> GetItemsPurchasesData(ItemsPurchasesRequest request, bool isPrint=false);
        Task<WebReport> ItemsPurchasesDataReport(ItemsPurchasesRequest request,exportType exportType,bool isArabic,int fileId=0);

    }
}
