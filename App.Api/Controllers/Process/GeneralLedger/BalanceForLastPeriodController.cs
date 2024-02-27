using App.Api.Controllers.BaseController;
using App.Application.Services.Process.BalanceForLastPeriods;
using App.Domain.Models.Security.Authentication.Request;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class BalanceForLastPeriodController : ApiGeneralLedgerControllerBase
    {
        private readonly IBalanceForLastPeriodBusiness balanceForLastPeriodBusiness;
        public BalanceForLastPeriodController(IBalanceForLastPeriodBusiness BalanceForLastPeriodBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            balanceForLastPeriodBusiness = BalanceForLastPeriodBusiness;
        }
        
        [HttpPost(nameof(CreateNewBalanceForLastPeriod))]
        public async Task<IRepositoryResult> CreateNewBalanceForLastPeriod([FromBody] BalanceForLastPeriodParameter parameter)
        {
            var add = await balanceForLastPeriodBusiness.AddBalanceForLastPeriod(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }

        [HttpGet("GetAllBalanceForLastPeriodDropDown")]
        public async Task<IRepositoryResult> GetAllBalanceForLastPeriodDropDown()
        {
            var account = await balanceForLastPeriodBusiness.GetAllBalanceForLastPeriodDropDown();
            var result = ResponseHandler.GetResult(account);
            return result;
        }
    }
}
