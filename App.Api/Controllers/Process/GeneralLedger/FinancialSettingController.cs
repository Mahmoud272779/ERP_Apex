using App.Api.Controllers.BaseController;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.FinancialSettings;
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
    public class FinancialSettingController : ApiGeneralLedgerControllerBase
    {
        private readonly IFinancialSettingBusiness financialSettingBusiness;
        public FinancialSettingController(IFinancialSettingBusiness FinancialSettingBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            financialSettingBusiness = FinancialSettingBusiness;
        }
        [HttpPost(nameof(CreateNewFinancialSetting))]
        public async Task<IRepositoryResult> CreateNewFinancialSetting([FromBody] FinancialSettingParameter parameter)
        {
            var add = await financialSettingBusiness.AddFinancialSetting(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpGet("GetFinancialSettingForBank")]
        public async Task<IRepositoryResult> GetFinancialSettingForBank()
        {
            var add = await financialSettingBusiness.GetFinancialSettingForBank();
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpGet("GetFinancialSettingForTreasury")]
        public async Task<IRepositoryResult> GetFinancialSettingForTreasury()
        {
            var add = await financialSettingBusiness.GetFinancialSettingForTreasury();
            var result = ResponseHandler.GetResult(add);
            return result;
        }
    }
}
