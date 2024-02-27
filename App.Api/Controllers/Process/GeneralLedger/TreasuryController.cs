using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Safes;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class TreasuryController : ApiGeneralLedgerControllerBase
    {
        private readonly ISafesBusiness treasuryBusiness;
        private readonly iAuthorizationService _iAuthorizationService;

        public TreasuryController(ISafesBusiness TreasuryBusiness,iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            treasuryBusiness = TreasuryBusiness;
            _iAuthorizationService = iAuthorizationService;
        }
        
        [HttpPost(nameof(CreateNewTreasury))]
        public async Task<ResponseResult> CreateNewTreasury([FromBody] TreasuryParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.AddTreasury(parameter);
            return result;
        }
        [HttpPut(nameof(UpdateTreasury))]
        public async Task<ResponseResult> UpdateTreasury(UpdateTreasuryParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.UpdateTreasury(parameter);
            return result;
        }
        
        [HttpPut(nameof(UpdateTreasuryStatus))]
        public async Task<ResponseResult> UpdateTreasuryStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.UpdateStatus(parameter);
            return result;
        }
        [HttpDelete("Deletetreasury")]
        public async Task<ResponseResult> Deletetreasury([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await treasuryBusiness.DeleteTreasuryAsync(parameter);
            return result;
        }
        [HttpGet("GetTreasuryById/{id}")]
        public async Task<ResponseResult> GetTreasuryById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.GetTreasuryById(id);
            return result;
        }
        
        [HttpGet("GetAllTreasury")]
        public async Task<ResponseResult> GetAllTreasury(int PageSize, int PageNumber, string? Name, int Status)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            PageTreasuryParameter parameters = new PageTreasuryParameter()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = Status
            };

            var result = await treasuryBusiness.GetAllTreasuryData(parameters);
            return result;
        }
        
        [HttpGet("GetAllTreasuryDropDown")]
        public async Task<ResponseResult> GetAllTreasuryDropDown()
        {
            var result = await treasuryBusiness.GetAllTreasuryDataDropDown();
            return result;
        }

        [HttpGet("GetAllTreasuryByUserDropDown")]
        public async Task<ResponseResult> GetAllTreasuryByUserDropDown()
        {
            var result = await treasuryBusiness.GetAllTreasuryByUserDropDown();
            return result;
        }
        [HttpGet("GetAllTreasuryWithoutFilterDropDown")]
        public async Task<ResponseResult> GetAllTreasuryWithoutFilterDropDown()
        {
            var result = await treasuryBusiness.GetAllTreasuryWithoutFilterDropDown();
            return result;
        }
        [HttpGet("GetAllTreasuryHistory")]
        public async Task<ResponseResult> GetAllTreasuryHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.GetAllTreasuryHistory(Id);
            return result;
        }
        [HttpGet("GetAllTreasurySetting")]
        
        public async Task<ResponseResult> GetAllTreasurySetting()
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.safes, (int)SubFormsIds.Safes_MainData, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await treasuryBusiness.GetAllTreasuryDataSetting();
            return result;
        }
    }
}
