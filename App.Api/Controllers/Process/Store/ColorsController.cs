using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
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
    public class ColorsController : ApiStoreControllerBase
    {
        private readonly IColorsService ColorsService;
        private readonly iAuthorizationService _iAuthorizationService;

        public ColorsController(IColorsService _ColorsService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            ColorsService = _ColorsService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddColor))]
        public async Task<ResponseResult> AddColor(ColorsParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ColorsService.AddColor(parameter);
            return result;
        }



        
        [HttpGet("GetListOfColors")]

        public async Task<ResponseResult> GetListOfColors(int PageNumber, int PageSize, string? Name, int status)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            ColorsSearch paramters = new ColorsSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Status = status

            };
            var result = await ColorsService.GetListOfColors(paramters);
            return result;
        }
        
        [HttpPut(nameof(UpdateColor))]
        public async Task<ResponseResult> UpdateColor(UpdateColorParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ColorsService.UpdateColors(parameters);
            return result;
        }
        
        [HttpPut(nameof(UpdateActiveColor))]
        public async Task<ResponseResult> UpdateActiveColor(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ColorsService.UpdateStatus(parameters);
            return result;
        }
        
        [HttpDelete("DeleteColors")]
        public async Task<ResponseResult> DeleteColors([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var result = await ColorsService.DeleteColors(parameter);
            return result;

        }

        
        [HttpGet("GetColorHistory")]
        public async Task<ResponseResult> GetColorHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Color_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await ColorsService.GetColorHistory(Id);
            return result;
        }

    }
}
