using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Commission_list;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class CommissionListController : ApiStoreControllerBase
    {
        private readonly ICommistionListService CommListService;
        private readonly iAuthorizationService _iAuthorizationService;

        public CommissionListController(ICommistionListService _CommListService ,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            CommListService = _CommListService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddCommissionList))]
        public async Task<ResponseResult> AddCommissionList(CommissionListRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.CommissionList_Sales, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CommListService.AddCommissionList(parameter);
            return result;
        }
        
        [HttpGet("GetCommissionList")]
        public async Task<ResponseResult> GetCommissionList(int PageNumber, int PageSize, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.CommissionList_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            CommissionListSearch parameter = new CommissionListSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,

            };
            var result = await CommListService.GetCommissionList(parameter);
            return result;
        }
        
        [HttpPut(nameof(UpdateCommissionList))]
        public async Task<ResponseResult> UpdateCommissionList(UpdateCommissionListRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.CommissionList_Sales, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CommListService.UpdateCommissionList(parameter);
            return result;
        }

        //
        //[HttpPost(nameof(UpdateActiveCommissionList))]
        //public async Task<ResponseResult> UpdateActiveCommissionList(UpdateActiveCommissionList parameter)
        //{
        //    var result = await CommListService.UpdateActiveCommissionList(parameter);
        //    return result;
        //}

        
        [HttpDelete("DeleteCommissionList")]
        public async Task<ResponseResult> DeleteCommissionList([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.CommissionList_Sales, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete codeList = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await CommListService.DeleteCommissionList(codeList);
            return result;
        }

        
        [HttpGet("GetCommissionListHistory")]
        public async Task<ResponseResult> GetCommissionListHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.CommissionList_Sales, Opretion.Print);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CommListService.GetCommissionListHistory(Id);
            return result;
        }

        
        [HttpGet("GetCommissionListDropDown")]
        public async Task<ResponseResult> GetCommissionListDropDown()
        {
            var result = await CommListService.GetCommissionListDropDown();
            return result;
        }
    }
}
