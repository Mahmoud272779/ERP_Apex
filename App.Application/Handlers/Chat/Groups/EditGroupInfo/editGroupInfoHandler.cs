using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Application.Handlers.Chat.Groups
{
    public class editGroupInfoHandler : IRequestHandler<editGroupInfoRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<chatGroups> _chatGroupsCommand;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<signalR> _SignalRQuery;
        private readonly IHubContext<NotificationHub> _hub;

        public editGroupInfoHandler(IRepositoryCommand<chatGroups> chatGroupsCommand, IRepositoryQuery<chatGroups> chatGroupsQuery, IRepositoryQuery<signalR> signalRQuery, IHubContext<NotificationHub> hub)
        {
            _chatGroupsCommand = chatGroupsCommand;
            _chatGroupsQuery = chatGroupsQuery;
            _SignalRQuery = signalRQuery;
            _hub = hub;
        }
        public async Task<ResponseResult> Handle(editGroupInfoRequest request, CancellationToken cancellationToken)
        {
            //check if group exist 
            var group = _chatGroupsQuery.TableNoTracking.Where(x => x.Id == request.groupId);
            if (!group.Any())
                return new ResponseResult()
                {
                    Note = "Group is not exist",
                    Result = Result.Failed
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            var selectedGroup = group.FirstOrDefault();
            if (userInfo.employeeId != selectedGroup.groupCreatorId)
                return new ResponseResult()
                {
                    Note = "Group Creator Only Can Edit the Group Info",
                    Result = Result.Failed
                };
            selectedGroup.groupName = request.groupName;
            selectedGroup.allowReply = request.allowReply;
            selectedGroup.canExit = request.canExit;
            _chatGroupsCommand.Update(selectedGroup);
            var saved = await _chatGroupsCommand.SaveAsync();
            return new ResponseResult()
            {
                Note = saved ? "success" : "failed",
                Result = saved ? Result.Success : Result.Failed,
            };
        }
    }
}
