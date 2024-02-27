using App.Application.Handlers.GeneralAPIsHandler.GetMobileAppVersion;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers
{
    public class MobileAppController : Controller
    {
        private readonly IMediator _mediator;

        public MobileAppController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [AllowAnonymous]
        [HttpGet("GetMobileAppVersion")]
        public  async Task<ResponseResult> GetMobileAppVersion()
        {
            var result = Defults.GetMobileAppVersion();
            return result;
        }
    }
}
