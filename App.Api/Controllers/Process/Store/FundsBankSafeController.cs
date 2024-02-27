using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Funds_Banks_and_Safes;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class FundsBankSafeController: ApiStoreControllerBase
    {
        private readonly IFundsBankSafeService FundsBankSafeService;
        private readonly iAuthorizationService _authorizationService;

        public FundsBankSafeController(IFundsBankSafeService _FundsBankSafeService ,
                        IActionResultResponseHandler ResponseHandler,iAuthorizationService authorizationService) : base(ResponseHandler)
        {
            FundsBankSafeService = _FundsBankSafeService;
            _authorizationService = authorizationService;
        }


        
        [HttpPost(nameof(AddFundsBankSafe))]
        public async Task<ResponseResult> AddFundsBankSafe(FundsBankSafeRequest parameter)
        {
            var isAuthorized = await _authorizationService.isAuthorized((int)MainFormsIds.ItemsFund,parameter.IsBank? (int)SubFormsIds.Banks_Fund : (int)SubFormsIds.Safes_Fund, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;

            var result = await FundsBankSafeService.AddFundsBankSafe(parameter);
            return result;
        }

        
        [HttpPut(nameof(UpdateFundsBankSafe))]
        public async Task<ResponseResult> UpdateFundsBankSafe(UpdateFundsBankSafeRequest parameter)
        {
            var isAuthorized = await _authorizationService.isAuthorized((int)MainFormsIds.ItemsFund, parameter.IsBank ? (int)SubFormsIds.Banks_Fund : (int)SubFormsIds.Safes_Fund, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;

            var result = await FundsBankSafeService.UpdateFundsBankSafe(parameter);
            return result;
        }

        
        [HttpDelete("DeleteFundsBankSafe")]
        public async Task<ResponseResult> DeleteFundsBankSafe([FromBody] Delete parameter)
        {
            var result = await FundsBankSafeService.DeleteFundsBankSafe(parameter);
            return result;
        }

        
        [HttpGet("GetListOfFundsBanksSafes")]
        public async Task<ResponseResult> GetListOfFundsBanksSafes([FromQuery] fundsSearch parameter)
        {
            var isAuthorized = await _authorizationService.isAuthorized((int)MainFormsIds.ItemsFund, parameter.IsBank ? (int)SubFormsIds.Banks_Fund : (int)SubFormsIds.Safes_Fund, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await FundsBankSafeService.GetListOfFundsBanksSafes(parameter);
            return result;
        }

        
        [HttpGet("GetFundsBankSafeHistory")]
        public async Task<ResponseResult> GetFundsBankSafeHistory(int Id)
        {
            
            var result = await FundsBankSafeService.GetFundsBankSafeHistory(Id);
            return result;
        }
        [HttpGet("GetFundsBanksSafesById/{Id}")]
        public async Task<ResponseResult> GetFundsBanksSafesById(int Id)
        {
            var result = await FundsBankSafeService.GetFundsBanksSafesById(Id);
            return result;
        }
    }
}
