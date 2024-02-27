using App.Api.Controllers.BaseController;
using App.Application.Handlers.Restaurants.Kitchens;
using App.Application.Handlers.Restaurants.Kitchens.GetListOfKitchens;
using App.Application.Handlers.Restaurants.Kitchens.UpdateKitchens;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.RestaurantsServices.KitchensServices;
using App.Application.Services.Process.RestaurantsServices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Application.Handlers.Restaurants;
using UpdateStatusRequest = App.Application.Handlers.Restaurants.UpdateStatusRequest;
using DeleteKitchensRequest = App.Application.Handlers.Restaurants.DeleteKitchensRequest;
using App.Application.Services.Process.Unit;

namespace App.Api.Controllers.Process.Restaurants
{
    public class KitchenController : ApiRestaurantControllerBase
    {
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly iKitchensServices kitchensServices;

        public KitchenController(iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler
            , iKitchensServices kitchensServices) : base(ResponseHandler)
        {
            _iAuthorizationService = iAuthorizationService;
            this.kitchensServices = kitchensServices;
        }

        [HttpPost("AddKitchens")]
        public async Task<ResponseResult> AddKitchens(AddKitchensRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await kitchensServices.AddKitchen(parameter);
            return add;
        }
        [HttpGet("GetListOfKitchens")]
        public async Task<ResponseResult> GetListOfKitchens(int PageNumber, int PageSize, int Status, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            GetListOfKitchensRequest parameters = new GetListOfKitchensRequest()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
            var add = await kitchensServices.GetListOfKitchens(parameters);
            return add;
        }
        [HttpPut("UpdateKitchens")]
        public async Task<ResponseResult> UpdateKitchens(UpdateKitchensRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await kitchensServices.UpdateKitchens(parameters);
            return add;
        }
        [HttpPut("UpdateActiveKitchens")]
        public async Task<ResponseResult> UpdateActiveKitchens(UpdateStatusRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await kitchensServices.UpdateStatus(parameters);
            return add;
        }
        [HttpDelete("DeleteKitchens")]
        public async Task<ResponseResult> DeleteKitchens([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteKitchensRequest parameter = new DeleteKitchensRequest()
            {
                Ids = Id
            };
            var add = await kitchensServices.DeleteKitchens(parameter);
            return add;

        }
        [HttpGet("GetKitchensHistory")]
        public async Task<ResponseResult> GetKitchensHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Restaurants, (int)SubFormsIds.Kitchens, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await kitchensServices.GetKitchensHistory(Id);
            return add;

        }
        [HttpGet("GetKitchensDropDown")]
        public async Task<ResponseResult> GetKitchensDropDown()
        {
            var add = await kitchensServices.GetKitchensDropDown();
            return add;

        }
    }
}
