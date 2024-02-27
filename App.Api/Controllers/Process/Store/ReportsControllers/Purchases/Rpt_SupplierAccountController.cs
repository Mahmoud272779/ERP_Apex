using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Supplier_Account;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.ReportsControllers.PurchasesControllers
{
    public class Rpt_SupplierAccountController : ApiControllerBase
    {
        private readonly ISupplierAccountService SupplierAccountService;
        public Rpt_SupplierAccountController(ISupplierAccountService SupplierAccountService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.SupplierAccountService = SupplierAccountService;
        }



        [AllowAnonymous]
        [HttpGet("GetSupplierAccountData1")]
        public async Task<ResponseResult> GetSupplierAccountData1(int supplierId,  [FromQuery] int[] branches, DateTime dateFrom, DateTime dateTo , bool PaidPurchase)
        {
            SupplierAccountRequest request = new SupplierAccountRequest()
            {
                SupplierId = supplierId,
                PaidPurchase = PaidPurchase,
                Branches = branches,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = await SupplierAccountService.GetSupplierAccountData(request);
            return result;
        }
    }
}
