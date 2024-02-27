using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Helper.GetCompanyConsumption;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.userManagmentApplication
{
    public class GetCompanyInfoController : ApiGeneralControllerBase
    {

        public GetCompanyInfoController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("GetCompanyConsumption")]
        public async Task<IActionResult> GetCompanyConsumption()
        {
            var res = await QueryAsync<List<GetCompanyConsumptionResponse>>(new GetCompanyConsumptionRequest());
            return Ok(res);
        }
    }
}
