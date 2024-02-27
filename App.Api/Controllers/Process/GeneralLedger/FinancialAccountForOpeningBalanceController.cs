using App.Api.Controllers.BaseController;
using App.Application.Services.Process.FinancialAccountForOpeningBalances;
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
    public class FinancialAccountForOpeningBalanceController : ApiGeneralLedgerControllerBase
    {
        private readonly IFinancialAccountForOpeningBalanceBusiness financialAccountBusiness;
        public FinancialAccountForOpeningBalanceController(IFinancialAccountForOpeningBalanceBusiness FinancialAccountBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            financialAccountBusiness = FinancialAccountBusiness;
        }
        [HttpPost(nameof(CreateNewFinancialAccountForOpeningBalance))]
        public async Task<IRepositoryResult> CreateNewFinancialAccountForOpeningBalance([FromBody] FinancialAccountForOpeningBalanceParameter parameter)
        {
            var add = await financialAccountBusiness.AddFinancialAccountForOpeningBalance(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpPut(nameof(UpdateFinancialAccountForOpeningBalance))]
        public async Task<IRepositoryResult> UpdateFinancialAccountForOpeningBalance(UpdateFinancialAccountForOpeningBalanceParameter parameter)
        {
            var add = await financialAccountBusiness.UpdateFinancialAccountForOpeningBalance(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
    }
}
