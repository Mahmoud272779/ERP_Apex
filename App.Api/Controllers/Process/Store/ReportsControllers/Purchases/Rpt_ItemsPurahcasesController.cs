using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases;
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
    public class Rpt_ItemsPurahcasesController: ApiControllerBase
    {
        private readonly IItemsPurchasesService ItemsPurchases;
        public Rpt_ItemsPurahcasesController(IItemsPurchasesService ItemsPurchases,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.ItemsPurchases = ItemsPurchases;
        }



        [AllowAnonymous]
        [HttpGet("GetItemsPurchasesData1")]
        public async Task<ResponseResult> GetItemsPurchasesData1(int? paymentMethod, int? itemId, int? itemType,
                      int? categoryId  ,[FromQuery] int[] branches,int? storeId, DateTime dateFrom, DateTime dateTo)
        {
            ItemsPurchasesRequest request = new ItemsPurchasesRequest()
            {
                paymentMethod = paymentMethod,
                itemId = itemId,
                itemType = itemType,
                categoryId = categoryId,
                Branches = branches,
                storeId = storeId,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var result = await ItemsPurchases.GetItemsPurchasesData(request);
            return result;
        }
    }
}
