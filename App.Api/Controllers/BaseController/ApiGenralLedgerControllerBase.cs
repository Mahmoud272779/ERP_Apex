using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.BaseController
{
    [Route("api/GeneralLedger/[controller]")]
    [ApiController]

    public class ApiGeneralLedgerControllerBase : ApiControllerBase
    {
        public ApiGeneralLedgerControllerBase(IActionResultResponseHandler responseHandler) : base(responseHandler)
        {

        }
    }
}
