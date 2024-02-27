using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
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
    public class Rpt_ItemPurchasesForSupplierController : ApiControllerBase
    {
        private readonly IItemPurchasesForSupplierService ItemPurchasesForSupplier;
        public Rpt_ItemPurchasesForSupplierController(IItemPurchasesForSupplierService ItemPurchasesForSupplier,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.ItemPurchasesForSupplier = ItemPurchasesForSupplier;
        }



        [AllowAnonymous]
        [HttpGet("GetItemPurchasesForSupplierData1")]
        public async Task<ResponseResult> GetItemPurchasesForSupplierData1(int supplierId , int itemId , [FromQuery] int[] branches , DateTime dateFrom , DateTime dateTo)
        {
            ItemPurchasesForSupplierRequest request = new ItemPurchasesForSupplierRequest()
            {
                SupplierId = supplierId ,
                ItemId = itemId,
                Branches = branches,
                DateFrom=dateFrom,
                DateTo=dateTo
            };
            var result = await ItemPurchasesForSupplier.GetItemPurchasesForSupplierData(request);
            return result;
        }
    }
}
