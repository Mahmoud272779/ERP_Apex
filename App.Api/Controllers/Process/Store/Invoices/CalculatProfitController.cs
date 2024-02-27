using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.Process;
using App.Application.Services.Process.Invoices;
using App.Domain.Models.Request.Store;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Invoices
{
    public class CalculatProfitController : ApiStoreControllerBase
    {
        private readonly ICalculateProfitService calcSystemService;

        public CalculatProfitController(ICalculateProfitService calcSystemService,
                      IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.calcSystemService = calcSystemService;
        }

        
        [HttpPost(nameof(CalculationOfInvoices))]  
        public async Task<IActionResult> CalculationOfInvoices(ProfitRequest parameter)
        {
            var result = await calcSystemService.CalculateAllProfit(parameter);
            return Ok(result);
        }
        [HttpGet(nameof(GetEditedItem))]
        public async Task<ResponseResult> GetEditedItem()
        {
            var result = await calcSystemService.GetEditData();
            return result;
        }
    }
}
