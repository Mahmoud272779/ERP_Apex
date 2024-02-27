using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Currancy;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class CurrencyController : ApiGeneralLedgerControllerBase
    {
        private readonly ICurrencyBusiness currencyBusiness;
        private readonly iAuthorizationService _iAuthorizationService;

        public CurrencyController(ICurrencyBusiness CurrencyBusiness
            , iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            currencyBusiness = CurrencyBusiness;
            _iAuthorizationService = iAuthorizationService;
        }
        [HttpGet("GenerateAutomaticCurrencyCode")]
        public async Task<int> GenerateAutomaticCurrencyCode()
        {
            var add = await currencyBusiness.AddAutomaticCode();
            return add;
        }
        [HttpPost(nameof(CreateNewCurrency))]
        public async Task<ResponseResult> CreateNewCurrency([FromBody] CurrencyParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.AddCurrency(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateCurrency))]
        public async Task<ResponseResult> UpdateCurrency(UpdateCurrencyParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.UpdateCurrency(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateCurrencyFactor))]
        public async Task<ResponseResult> UpdateCurrencyFactor(UpdateCurrencyFactorList parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.UpdateCurrencyFactor(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateCurrencyStatus))]
        public async Task<ResponseResult> UpdateCurrencyStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.UpdateCurrencyStatus(parameter);
            return result;
        }
        [HttpDelete("DeleteCurrency")]
        public async Task<ResponseResult> DeleteCurrency([FromBody] int[] Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };
            var result = await currencyBusiness.DeleteCurrencyAsync(parameter);
            return result;
        }
        [HttpGet("GetCurrencyById/{id}")]
        public async Task<ResponseResult> GetCurrencyById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.GetCurrencyById(id);
            return result;
        }
        [HttpGet("GetAllCurrency")]
        public async Task<ResponseResult> GetAllCurrency(int PageNumber, int PageSize, string? Name , int Status)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            currencyRequest parameters = new currencyRequest()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = Name,
                Status = Status
            };
            var result = await currencyBusiness.GetAllCurrencyData(parameters);
            return result;
        }
        [HttpGet("GetAllCurrencyDropDown")]
        public async Task<ResponseResult> GetAllCurrencyDropDown()
        {
            var result = await currencyBusiness.GetAllCurrencyDataDropDown();
            return result;
        }

        [HttpGet("GetAllCurrencyHistory")]
        public async Task<ResponseResult> GetAllCurrencyHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Currencies_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await currencyBusiness.GetAllCurrencyHistory(Id);
            return result;
        }
    }
}
