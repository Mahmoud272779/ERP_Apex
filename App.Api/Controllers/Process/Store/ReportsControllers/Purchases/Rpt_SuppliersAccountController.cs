using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers.PurchasesControllers
{
    public class Rpt_SuppliersAccountController: ApiControllerBase
    {
        private readonly ISuppliersAccountService SuppliersAccountService;
        public Rpt_SuppliersAccountController(ISuppliersAccountService SuppliersAccountService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.SuppliersAccountService = SuppliersAccountService;
        }



        [AllowAnonymous]
        [HttpGet("GetSuppliersAccountData1")]
        public async Task<ResponseResult> GetSuppliersAccountData1( [FromQuery] int[] branches, DateTime dateFrom, DateTime dateTo, bool zerosPrices)
        {
            SuppliersAccountRequest request = new SuppliersAccountRequest()
            {
                zerosPrices = zerosPrices, 
                Branches = branches,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = await SuppliersAccountService.GetSuppliersAccountData(request);
            return result;
        }
    }
}
