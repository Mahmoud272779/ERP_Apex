using App.Domain.Enums;
using App.Domain.Models.Request;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Services.Reports.Invoices.Purchases.Supplier_Account
{
    public interface IPersonAccountService
    {
        Task<ResponseResult> GetPersonAccountData(SupplierAccountRequest request,bool isPrint = false);
        Task<WebReport> GetPersonAccountDataReport(SupplierAccountRequest request, exportType exportType, bool isArabic, int fileId = 0);


    }
}
