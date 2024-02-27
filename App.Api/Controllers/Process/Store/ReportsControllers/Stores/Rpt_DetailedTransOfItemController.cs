using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers.Stores
{
    public class Rpt_DetailedTransOfItemController : ApiControllerBase
    {
        private readonly IDetailedTransOfItemService DetailedTransOfItemService;

        public Rpt_DetailedTransOfItemController(IDetailedTransOfItemService DetailedTransOfItemService,
                  IActionResultResponseHandler responseHandler):base(responseHandler)
        {
            this.DetailedTransOfItemService = DetailedTransOfItemService;
        }
        [AllowAnonymous]
        [HttpGet("DetailedTransactoinsOfItem1")]
        public async Task<ResponseResult> DetailedTransactoinsOfItem1( int itemId , int unitId  , int storeId , DateTime dateFrom , DateTime dateTo)
        {
            var request = new DetailedTransOfItemRequest()
            {
                itemId = itemId,
                unitId = unitId,
                storeId = storeId,
                dateFrom = dateFrom,
                dateTo = dateTo
            };
            var res =await DetailedTransOfItemService.DetailedTransactoinsOfItem(request);
            return res;
        }
    }
}
