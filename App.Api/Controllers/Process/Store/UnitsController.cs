using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Unit;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class UnitsController : ApiStoreControllerBase
    {
        private readonly IUnitsService UnitsService;
        private readonly iAuthorizationService _iAuthorizationService;

        public UnitsController(IUnitsService _UnitsService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            UnitsService = _UnitsService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPost(nameof(AddUnits))]
        public async Task<ResponseResult> AddUnits(AddUnitRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await UnitsService.AddUnit(parameter);
            return add;
        }
        [HttpGet("GetListOfUnits")]
        public async Task<ResponseResult> GetListOfUnits(int PageNumber, int PageSize, int Status, string? Name)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            GetListOfUnitsRequest parameters = new GetListOfUnitsRequest()
            {

                PageNumber = PageNumber,
                PageSize = PageSize,
                Status = Status,
                Name = Name

            };
            var add = await UnitsService.GetListOfUnits(parameters);
            return add;
        }
        [HttpPut(nameof(UpdateUnits))]
        public async Task<ResponseResult> UpdateUnits(UpdateUnitsRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await UnitsService.UpdateUnits(parameters);
            return add;
        }
        [HttpPut(nameof(UpdateActiveUnits))]
        public async Task<ResponseResult> UpdateActiveUnits(UpdateStatusRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await UnitsService.UpdateStatus(parameters);
            return add;
        }
        [HttpDelete("DeleteUnits")]
        public async Task<ResponseResult> DeleteUnits([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            DeleteKitchensRequest parameter = new DeleteKitchensRequest()
            {
                Ids = Id
            };
            var add = await UnitsService.DeleteUnits(parameter);
            return add;                                                 

        }        
        [HttpGet("GetUnitHistory")]
        public async Task<ResponseResult> GetUnitHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Units_MainUnits, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await UnitsService.GetUnitHistory(Id);
            return add;

        }        
        [HttpGet("GetUnitsDropDown")]
        public async Task<ResponseResult> GetUnitsDropDown()
        {
            var add = await UnitsService.GetUnitsDropDown();
            return add;

        }
        [HttpGet("GetUnitsByDate")]
        public async Task<ResponseResult> GetUnitsByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            GetUnitsByDateRequest parameters = new GetUnitsByDateRequest()
            {
                date = date,
                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var units = await UnitsService.GetUnitsByDate(parameters);
            return units;

        }

    }
}
