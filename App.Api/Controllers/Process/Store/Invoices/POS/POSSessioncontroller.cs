using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.POS;
using App.Application.Handlers.POS.checkUserSessionStatus;
using App.Application.Handlers.POS.ClosePOSSession;
using App.Application.Handlers.POS.closeSession;
using App.Application.Handlers.POS.GetMyCurrentPOSSession;
using App.Application.Handlers.POS.GetPOSSessionHistory;
using App.Application.Handlers.POS.GetSessionsOpened;
using App.Application.Handlers.POS.OpenPOSSeassons;
using App.Application.Handlers.POS.resumeSession;
using App.Application.Handlers.POS.SessionBinding;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.POS;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api
{
    public class POSSessioncontroller : ApiStoreControllerBase
    {
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;

        public POSSessioncontroller(
                                    iAuthorizationService iAuthorizationService,
                                    IActionResultResponseHandler ResponseHandler,
                                    IMediator mediator,
                                    IPosService posService) : base(ResponseHandler)
        {
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
        }
        [HttpPost("OpenPOSSeassion")]
        public async Task<ResponseResult> OpenPOSSeassion()
        {
            var openSeasson = await _mediator.Send(new openPOSSessionRequest());
            return openSeasson;
        }
        [HttpPost("ClosePOSSeassion/{sessionId}")]
        public async Task<ResponseResult> ClosePOSSeassion(int sessionId)
        {
            var openSeasson = await _mediator.Send(new closePOSSessionRequest() { sessionId = sessionId });
            return openSeasson;
        }
        [HttpGet("GetPOSSessions")]
        public async Task<ResponseResult> GetPOSSessions([FromQuery] GetOpenSessionsPOSRequest request)
        {
            var isAuthoized = await _iAuthorizationService.isAuthorized(0, (int)SubFormsIds.POSSession, Domain.Enums.Opretion.Open);
            if (isAuthoized != null)
                return isAuthoized;
            var openSeasson = await _mediator.Send(request);
            return openSeasson;
        }
        [HttpGet("checkUserSessionStatus")]
        public async Task<ResponseResult> checkUserSessionStatus()
        {
            var openSeasson = await _mediator.Send(new checkUserSessionStatusRequest() { });
            return openSeasson;
        }
        [HttpGet("bindSession/{sessionId}")]
        public async Task<ResponseResult> bindSession(int sessionId)
        {
            var openSeasson = await _mediator.Send(new SessionBindingRequest() {sessionId = sessionId });
            return openSeasson;
        }
        [HttpGet("resumeSession/{sessionId}")]
        public async Task<ResponseResult> resumeSession(int sessionId)
        {
            var openSeasson = await _mediator.Send(new resumeSessionRequest() {sessionId = sessionId });
            return openSeasson;
        }
        [HttpGet("currentPOSsession")]
        public async Task<ResponseResult> currentPOSsession()
        {
            var openSeasson = await _mediator.Send(new currentPOSsessionRequest());
            return openSeasson;
        }
        [HttpGet("POSSessionDetalies/{SessionId}")]
        public async Task<ResponseResult> POSSessionDetalies(int SessionId)
        {
            var openSeasson = await _mediator.Send(new POSSessionDetaliesRequest { SessionId = SessionId });
            return openSeasson;
        }
        [HttpGet("POSSessionHistory/{SessionId}")]
        public async Task<ResponseResult> POSSessionHistory(int SessionId)
        {
            var openSeasson = await _mediator.Send(new getPOSSessionHistoryRequest { sessionId = SessionId });
            return openSeasson;
        }



    }
}

