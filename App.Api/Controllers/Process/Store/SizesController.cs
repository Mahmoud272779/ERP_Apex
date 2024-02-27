using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Size;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class SizesController : ApiStoreControllerBase
    {
        private readonly ISizesService SizesService;
        private readonly iAuthorizationService _iAuthorizationService;

        public SizesController(ISizesService _SizesService, iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            SizesService = _SizesService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddSizes))]
        public async Task<ResponseResult> AddSizes(SizesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0,(int)SubFormsIds.Sizes_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SizesService.AddSize(parameter);
            return add;
        }

        
        [HttpGet("GetListOfSizes")]

        public async Task<ResponseResult> GetListOfSizes(int PageNumber, int PageSize, int Status, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Sizes_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            SizesSearch parameters = new SizesSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
            var add = await SizesService.GetListOfSizes(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateSizes))]
        public async Task<ResponseResult> UpdateSizes(UpdateSizesParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Sizes_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SizesService.UpdateSizes(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateActiveSizes))]
        public async Task<ResponseResult> UpdateActiveSizes(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Sizes_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SizesService.UpdateStatus(parameters);
            return add;
        }


        
        [HttpDelete("DeleteSizes")]
        public async Task<ResponseResult> DeleteSizes([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Sizes_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var add = await SizesService.DeleteSizes(ListCode);
            return add;

        }
        
        [HttpGet("GetSizeHistory")]
        public async Task<ResponseResult> GetSizeHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.Sizes_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SizesService.GetSizeHistory(Id);
            return add;

        }

    }
}
