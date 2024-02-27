using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process;
using App.Application.Services.Process.Color;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class OtherAuthoritiesController : ApiGeneralLedgerControllerBase
    {
        private readonly IOtherAuthoritiesServices OtherAuthoritiesService;
        private readonly iAuthorizationService _iAuthorizationService;

        public OtherAuthoritiesController(IOtherAuthoritiesServices _OtherAuthoritiesService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            OtherAuthoritiesService = _OtherAuthoritiesService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddOtherAuthorities))]
        public async Task<ResponseResult> AddOtherAuthorities(OtherAuthoritiesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await OtherAuthoritiesService.AddOtherAuthorities(parameter);
            return result;
        }

        
        [HttpGet("GetAllOtherAuthorities")]

        public async Task<ResponseResult> GetAllOtherAuthorities(int PageNumber, int PageSize, string? Name ,string? SearchCriteria, int? status)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            PageOtherAuthoritiesParameter paramters = new PageOtherAuthoritiesParameter()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = status,
                SearchCriteria = SearchCriteria

            };
            var result = await OtherAuthoritiesService.GetAllOtherAuthoritiesData(paramters);
            return result;
        }
        
        [HttpPut(nameof(UpdateOtherAuthorities))]
        public async Task<ResponseResult> UpdateOtherAuthorities(UpdateOtherAuthoritiesParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await OtherAuthoritiesService.UpdateOtherAuthorities(parameters);
            return result;
        }

        
        [HttpPut(nameof(UpdateOtherAuthoritieStatus))]
        public async Task<ResponseResult> UpdateOtherAuthoritieStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await OtherAuthoritiesService.UpdateStatus(parameters);
            return result;
        }
       
        [HttpGet("GetAllOtherAuthoritiesDropDown")]
        
        public async Task<ResponseResult> GetAllOtherAuthoritiesDropDown([FromQuery]DropDownParameter parameter)
        {
            var result = await OtherAuthoritiesService.GetAllOtherAuthoritiesDataDropDown( parameter);
            return result;
        }
        [HttpGet("GetOtherAuthoritieById/{id}")]
        public async Task<ResponseResult> GetOtherAuthoritieById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await OtherAuthoritiesService.GetOtherAuthoritiesById(id);
            return result;
        }

        [HttpGet("GetOtherAuthorityHistory")]
        public async Task<ResponseResult> GetOtherAuthorityHistory(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await OtherAuthoritiesService.GetOtherAuthoritiesHistory(id);
            return result;
        }

        [HttpDelete("DeleteOtherAuthorities")]
        public async Task<ResponseResult> DeleteOtherAuthorities([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.OtherAuthorities_MainData, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await OtherAuthoritiesService.DeleteOtherAuthoritiesAsync(parameter);
            return result;

        }
    }
}
