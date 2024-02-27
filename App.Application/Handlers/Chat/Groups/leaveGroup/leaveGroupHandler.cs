using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Application.Handlers.Chat.Groups
{
    public class leaveGroupHandler : IRequestHandler<leaveGroupRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<signalR> _signalR;
        private readonly IRepositoryQuery<chatGroups> _chatGroupQuery;
        private readonly IRepositoryCommand<chatGroups> _chatGroupCommand;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryCommand<chatGroupMembers> _chatGroupMembersCommand;
        private readonly IHubContext<NotificationHub> _hub;

        public leaveGroupHandler(iUserInformation iUserInformation, IRepositoryQuery<signalR> signalR, IRepositoryQuery<chatGroups> chatGroupQuery, IRepositoryCommand<chatGroups> chatGroupCommand, IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery, IRepositoryCommand<chatGroupMembers> chatGroupMembersCommand, IHubContext<NotificationHub> hub)
        {
            _iUserInformation = iUserInformation;
            _signalR = signalR;
            _chatGroupQuery = chatGroupQuery;
            _chatGroupCommand = chatGroupCommand;
            _chatGroupMembersQuery = chatGroupMembersQuery;
            _chatGroupMembersCommand = chatGroupMembersCommand;
            _hub = hub;
        }
        public async Task<ResponseResult> Handle(leaveGroupRequest request, CancellationToken cancellationToken)
        {
            //check is group exist
            var group = _chatGroupQuery.TableNoTracking.Where(x => x.Id == request.groupId);
            if (!group.Any())
                return new ResponseResult()
                {
                    Note = "Group is not exist",
                    Result = Result.Failed
                };
            //check if group allowed to leave
            if (!group.FirstOrDefault().canExit)
                return new ResponseResult()
                {
                    Note = "Group is not allow to leave",
                    Result = Result.Failed
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            var groupMembers = _chatGroupMembersQuery.TableNoTracking.Where(x => x.groupId == request.groupId);
            //check is user exist in the group
            if (!groupMembers.Where(x => x.memberId == userInfo.employeeId).Any())
                return new ResponseResult()
                {
                    Note = $"You are not exist in group {group.FirstOrDefault().groupName}",
                    Result = Result.Failed
                };
            var currentMember = groupMembers.Where(x => x.memberId == userInfo.employeeId).FirstOrDefault();
            _chatGroupMembersCommand.Remove(currentMember);
            var saved = await _chatGroupMembersCommand.SaveAsync();
            if (saved)
            {
                var groupMembersConnectionId = _signalR.TableNoTracking.Where(x => groupMembers.Select(c => c.memberId).ToArray().Contains(x.InvEmployeesId)).Select(x => x.connectionId).ToList();
                var signalrResponse = new responseSignalR()
                {
                    groupId = request.groupId,
                    memberId = userInfo.employeeId,
                    noteAr = $"{userInfo.employeeNameAr} قام بالمغادرة",
                    noteEn = $"{userInfo.employeeNameEn} Has left the group",
                };
                await _hub.Clients.Clients(groupMembersConnectionId).SendAsync(defultData.leftGroup, signalrResponse);
            }
            return new ResponseResult()
            {
                Note = saved ? "group leaving success":"group leaving Faild",
                Result = saved ? Result.Success : Result.Failed
            };
        }
    }
    public class responseSignalR
    {
        public string noteAr { get; set; }
        public string noteEn { get; set; }
        public int groupId { get; set; }
        public int memberId { get; set; }
    }
}
