using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.CostCenters;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class CostCentersController : ApiGeneralLedgerControllerBase
    {
        private readonly ICostCentersBusiness costCenterBusiness;
        private readonly iAuthorizationService _iAuthorizationService;

        public CostCentersController(ICostCentersBusiness CostCenterBusiness,iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            costCenterBusiness = CostCenterBusiness;
            _iAuthorizationService = iAuthorizationService;
        }
        [HttpPost(nameof(CreateNewCostCenter))]
        public async Task<IRepositoryResult> CreateNewCostCenter([FromBody] CostCenterParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Add);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var add = await costCenterBusiness.AddCostCenter(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpPut(nameof(UpdateCostCenter))]
        public async Task<IRepositoryResult> UpdateCostCenter(UpdateCostCenterParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await costCenterBusiness.UpdateCostCenter(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpDelete("DeleteCostCenter")]
        public async Task<IActionResult> DeleteCostCenter([FromBody] int[] Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Delete);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };

            var res = await costCenterBusiness.DeleteCostCenterAsync(parameter);
            if (res.Result == Result.Success)
                return Ok(res);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, res);

        }
        [HttpGet("GetCostCenterById/{id}")]
        public async Task<IRepositoryResult> GetCostCenterById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await costCenterBusiness.GetCostCenterById(id);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllCost")]
        public async Task<IRepositoryResult> GetAllCost(int PageNumber, int PageSize,string? SearchCriteria)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            PageParameter parameters = new PageParameter()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria= SearchCriteria
            };
            var account = await costCenterBusiness.GetAllCostCenterData(parameters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllCostWitoutPagination")]
        public async Task<IRepositoryResult> GetAllCostWitoutPagination()
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await costCenterBusiness.GetAllCostCenterDataWithOutPage();
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllCostDropDown")]
        public async Task<IRepositoryResult> GetAllCostDropDown(int PageNumber, int PageSize, string? SearchCriteria)
        {
            PageParameter parameters = new PageParameter()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = SearchCriteria
            };
            var account = await costCenterBusiness.GetAllCostCenterDataDropDown(parameters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllCostCenterHistory/{CostCenerId}")]
        public async Task<ResponseResult> GetAllCostCenterHistory(int CostCenerId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CostCenter_GL, Opretion.Open);
            if (isAuthorized != null)
              return  isAuthorized;
               // return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await costCenterBusiness.GetAllCostCenterHistory(CostCenerId);
           // var result = ResponseHandler.GetResult(account);
            return account;
        }
        [HttpGet("GetAllCostCenterDropDownNoPaging")]
        public async Task<IRepositoryResult> GetAllCostCenterDropDownNoPaging()
        {
            var account = await costCenterBusiness.GetAllCostCenterDataDropDown();
            var result = ResponseHandler.GetResult(account);
            return result;
        }

        [HttpGet("GetAllCostCenterDropDown")]
        public async Task<IRepositoryResult> GetAllCostCenterDropDown(int type, int? finanncialAccountId)
        {
            var account = await costCenterBusiness.GetAllCostCenterDropDown(type, finanncialAccountId);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
    }
}
