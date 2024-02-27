using App.Api.Controllers.BaseController;
using App.Application.Handlers.Chat.Broadcast;
using App.Application.Handlers.NotificationSystem.GetUserNotifications;
using App.Application.Handlers.NotificationSystem.InsertNotification;
using App.Application.Handlers.NotificationSystem.MarkAllAsSeenServices;
using App.Application.Handlers.NotificationSystem.NotificationSeen;
using App.Domain.Models.Request.General;
using App.Infrastructure.settings;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;


namespace App.Api.Controllers.NotificationSystem
{

    public class NotificationSystemController : ApiGeneralControllerBase
    {

        private readonly IMediator _mediator;
        public NotificationSystemController(IMediator mediator,IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("BroadCast")]
        public async Task<IActionResult> BroadCast([FromBody]broadcastRequest request)
        {
            var res = await _mediator.Send(request);
            return Ok(res);
        }


        [HttpGet("GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] GetUserNotificationsRequest request)
        {
            var res = await _mediator.Send(request);
            return Ok(res);
        }


        [HttpPost("InsertNotification")]
        public async Task<IActionResult> InsertNotification([FromBody] NotificationRequestDTO request)
        {
            var res = await _mediator.Send(new InsertNotificationRequest
            {
                cDate = DateTime.Now,
                Desc = request.Desc,
                DescEn = request.DescEn,
                isSystem = false,
                specialUserId = request.specialUserId,
                title = request.title,
                titleEn = request.titleEn,
                notificationTypeId = request.notificationTypeId
            });
            return Ok(res);
        }
        [HttpPost("NotificationSeen")]
        public async Task<IActionResult> NotificationSeen([FromBody] NotificationSeenRequest request)
        {
            var res = await _mediator.Send(request);
            return Ok(res);
        }
        [HttpPost("MarkAllAsSeen")]
        public async Task<IActionResult> MarkAllAsSeen([FromBody] MarkAllAsSeenRequest request)
        {
            var res = await _mediator.Send(request);
            return Ok(res);
        }

    }
}
