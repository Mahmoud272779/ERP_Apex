using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Api.Controllers.BaseController
{
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.None,NoStore = true)]
    [Route("api")]
    [ApiController]    
    [Authorize]

    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Token Business
        /// </summary>

        /// <summary>
        /// Response Handler
        /// </summary>
        protected readonly IActionResultResponseHandler ResponseHandler;

        /// <inheritdoc />
        public ApiControllerBase(IActionResultResponseHandler responseHandler)
        {
            ResponseHandler = responseHandler;
        }

        /// <inheritdoc />
        private readonly IMediator _mediator;

        public ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException();
        }

        protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
        {
            return await _mediator.Send(query);
        }

        protected ActionResult<T> Single<T>(T data)
        {
            if (data == null) return NotFound();
            return Ok(data);
        }

        protected async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command)
        {
            return await _mediator.Send(command);
        }

    }
}
