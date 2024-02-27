using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.BaseController
{
    [Route("api/General/[controller]")]
    [ApiController]
    public class ApiGeneralControllerBase : ApiControllerBase
    {
        public ApiGeneralControllerBase(IActionResultResponseHandler responseHandler) : base(responseHandler)
        {

        }

        public ApiGeneralControllerBase(IMediator mediator) : base(mediator)
        {
        }
    }
}
