using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.Invoices.Purchases.Items_Purchases
{
    public interface IItemPurchasesForSupplierService
    {
          Task<ResponseResult> GetItemPurchasesForSupplierData(ItemPurchasesForSupplierRequest request,bool isPrint=false);
        Task<WebReport> ItemPurchasesForSupplierReport(ItemPurchasesForSupplierRequest request, exportType exportType, bool isArabic, int fileId=0);

    }
}
