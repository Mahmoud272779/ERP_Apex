using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier
{
    public interface  IItemsPurchasesForSupplierService
    {
        Task<ResponseResult> GetItemsPurchasesForSupplierData(ItemsPurchasesForSupplierRequest request,bool isPrint=false);
        Task<WebReport> ItemsPurchasesForSupplierReport(ItemsPurchasesForSupplierRequest request, exportType exportType, bool isArabic, int fileId = 0);

    }
}
