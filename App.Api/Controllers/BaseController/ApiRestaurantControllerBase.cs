using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.BaseController
{
    [Route("api/Restaurants/[controller]")]
    [ApiController]
    public class ApiRestaurantControllerBase : ApiControllerBase
    {
        public ApiRestaurantControllerBase(IActionResultResponseHandler responseHandler) : base(responseHandler)
        {

        }

        public ApiRestaurantControllerBase(IMediator mediator) : base(mediator)
        {
        }
    }
}
