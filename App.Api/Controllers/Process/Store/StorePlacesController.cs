using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Store_places;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class StorePlacesController : ApiStoreControllerBase
    {
        private readonly IStorePlacesService StorePlacesService;
        private readonly iAuthorizationService _iAuthorizationService;

        public StorePlacesController(IStorePlacesService _StorePlacesService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            StorePlacesService = _StorePlacesService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddStorePlaces))]
        public async Task<ResponseResult> AddStorePlaces(StorePlacesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StorePlacesService.AddStorePlace(parameter);
            return add;
        }



        
        [HttpGet("GetListOfStorePlaces")]

        public async Task<ResponseResult> GetListOfStorePlaces(int PageNumber, int PageSize, int Status, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            StorePlacesSearch parameters = new StorePlacesSearch()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
            var add = await StorePlacesService.GetListOfStorePlaces(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateStorePlaces))]
        public async Task<ResponseResult> UpdateStorePlaces(UpdateStorePlacesParameter parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StorePlacesService.UpdateStorePlaces(parameters);
            return add;
        }

        
        [HttpPut(nameof(UpdateActiveStorePlaces))]
        public async Task<ResponseResult> UpdateActiveStorePlaces(SharedRequestDTOs.UpdateStatus parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StorePlacesService.UpdateStatus(parameters);
            return add;
        }



        
        [HttpDelete("DeleteStorePlaces")]
        public async Task<ResponseResult> DeleteStorePlaces([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var add = await StorePlacesService.DeleteStorePlaces(ListCode);
            return add;

        }

        
        [HttpGet("GetStorePlaceHistory")]
        public async Task<ResponseResult> GetStorePlaceHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.StorePlaces_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await StorePlacesService.GetStorePlaceHistory(Id);
            return add;

        }

        
        [HttpGet("GetStorePlacesDropDown")]
        public async Task<ResponseResult> GetStorePlacesDropDown()
        {
            var result = await StorePlacesService.GetStorePlacesDropDown();
            return result;
        }

        
        [HttpGet("GetAllStorePlacesDropDown")]
        public async Task<ResponseResult> GetAllStorePlacesDropDown()
        {
            var result = await StorePlacesService.GetAllStorePlacesDropDown();
            return result;
        }
    }
}
