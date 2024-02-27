using App.Api.Controllers.BaseController;
using App.Application.Handlers.Chat.Groups;
using App.Application.Handlers.Chat.PrivateChat.ShowPrivateMessage;
using App.Application.Handlers.PrivateChat.Chat;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class ChatController : ApiGeneralLedgerControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator,
             IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            _mediator = mediator;
        }

        #region private Chat
        [HttpPost(nameof(SendPrivateChatMessage))]
        public async Task<ResponseResult> SendPrivateChatMessage(PrivateChatRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpGet(nameof(showPrivateMessages))]
        public async Task<ResponseResult> showPrivateMessages([FromQuery]showPrivateMessagesRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        #endregion
        #region groups
        [HttpPost(nameof(createGroup))]
        public async Task<ResponseResult> createGroup([FromForm]createGroupRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpPost(nameof(addGroupMembers))]
        public async Task<ResponseResult> addGroupMembers(addGroupMembersRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpPut(nameof(editGroupInfo))]
        public async Task<ResponseResult> editGroupInfo(editGroupInfoRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpPut(nameof(leaveGroup))]
        public async Task<ResponseResult> leaveGroup(leaveGroupRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpGet(nameof(openGroupChat))]
        public async Task<ResponseResult> openGroupChat([FromQuery]openGroupChatRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpPut(nameof(removeGroupMember))]
        public async Task<ResponseResult> removeGroupMember(removeGroupMemberRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        [HttpPost(nameof(sendGroupMessage))]
        public async Task<ResponseResult> sendGroupMessage(sendGroupMessageRequest parm)
        {
            var response = await _mediator.Send(parm);
            return response;
        }
        #endregion
    }
}
