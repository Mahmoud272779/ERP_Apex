using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.Items_Prices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.ReportsControllers.Items_Prices
{
    public class Rpt_ItemsPricesController : ApiStoreControllerBase
    {
        private readonly IRpt_ItemsPrices ItemsPrices;
        private readonly iAuthorizationService _iAuthorizationService;

        public Rpt_ItemsPricesController(IRpt_ItemsPrices _ItemsPrices,
                        IActionResultResponseHandler ResponseHandler,iAuthorizationService iAuthorizationService) : base(ResponseHandler)
        {
            ItemsPrices = _ItemsPrices;
            this._iAuthorizationService = iAuthorizationService;
        }

     

      
      [HttpGet("GetByFiltersWithPagenation")]
      public async Task<ResponseResult> GetByFiltersWithPagenation(string itemCode  , string itemName , int? categoryId , int? status ,int? typeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.SalesBranchProfit, Opretion.Open);
            if (isAuthorized != null) return isAuthorized;

            var Request = new ItemsPricesRequest 
            {
                ItemCode = itemCode,
                ItemName = itemName,
                CategoryId = categoryId,
                Status = status,
                TypeId=typeId
            };
            var result = await ItemsPrices.GetByFiltersWithPagenation(Request);
            return result;
        }
    }
}
