using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.BaseController
{
    [Route("api/Store/[controller]")]
    [ApiController]
    public class ApiStoreControllerBase : ApiControllerBase
    {
        public ApiStoreControllerBase(IActionResultResponseHandler responseHandler) : base(responseHandler)
        {

        }

        public ApiStoreControllerBase(IMediator mediator) : base(mediator)
        {
        }
    }
}
