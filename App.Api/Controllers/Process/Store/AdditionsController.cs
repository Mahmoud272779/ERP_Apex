using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Additions;
using App.Application.Handlers.Additions.AddAdditions;
using App.Application.Handlers.Additions.DeleteAdditions;
using App.Application.Handlers.Additions.GetAdditionsHistory;
using App.Application.Handlers.Additions.UpdateAdditions;
using App.Application.Handlers.Units;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Unit;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class AdditionsController : ApiStoreControllerBase
    {
         private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator mediator;

        public AdditionsController(iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler,
                        IMediator mediator) : base(ResponseHandler)
        {
            _iAuthorizationService = iAuthorizationService;
            this.mediator = mediator;
        }


        [HttpPost(nameof(AddAdditions))]
        public async Task<ResponseResult> AddAdditions(AddAdditionsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await mediator.Send(parameter);
            return add;
        }

        [HttpPut(nameof(UpdateAdditions))]
        public async Task<ResponseResult> UpdateAdditions(UpdateAdditionsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await mediator.Send(parameter);
            return add;
        }

          [HttpGet("GetListOfAdditions")]
          public async Task<ResponseResult> GetListOfAdditions(int PageNumber, int PageSize, int Status, string? Name)
          {
              var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Open);
              if (isAuthorized != null)
                  return isAuthorized;
              GetListOfAdditionsRequest request = new GetListOfAdditionsRequest()
              {

                  PageNumber = PageNumber,
                  PageSize = PageSize,
                  Status = Status,
                  Name = Name

              };
            var add = await mediator.Send(request);
              return add;
          }


       

    
           [HttpDelete("DeleteAdditions")]
            public async Task<ResponseResult> DeleteAdditions([FromBody] int[] Id)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
                DeleteAdditionsRequest parameter = new DeleteAdditionsRequest()
                {
                    Ids = Id
                };
            var add = await mediator.Send(parameter);
                return add;                                                 

            }  
          [HttpGet("GetAdditionsHistory")]
          public async Task<ResponseResult> GetAdditionsHistory(int Id)
          {
              var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Open);
              if (isAuthorized != null)
                  return isAuthorized;

            GetAdditionsHistoryRequest req = new GetAdditionsHistoryRequest() { Code=Id};
            var add =await  mediator.Send(req);
              return add;

          }
        
          [HttpGet("GetAdditionsDropDown")]
           public async Task<ResponseResult> GetAdditionsDropDown()
           {
            var add = await mediator.Send(new GetAdditionsDropDownRequest());
               return add;

           }
        /*  [HttpGet("GetUnitsByDate")]
          public async Task<ResponseResult> GetUnitsByDate(DateTime date)
          {
              var units = await UnitsService.GetUnitsByDate(date);
              return units;

          }*/

        [HttpPut(nameof(UpdateActiveAdditions))]
        public async Task<ResponseResult> UpdateActiveAdditions(UpdateAdditionStatusRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.Additions, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await mediator.Send(parameters);
            return add;
        }


    }
}
