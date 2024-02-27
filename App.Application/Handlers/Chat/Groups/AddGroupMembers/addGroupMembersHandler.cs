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
    public class addGroupMembersHandler : IRequestHandler<addGroupMembersRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryCommand<chatGroupMembers> _chatGroupMembersCommand;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IHubContext<NotificationHub> _hub;


        public addGroupMembersHandler(iUserInformation iUserInformation, 
                                      IRepositoryQuery<signalR> signalRQuery, 
                                      IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery, 
                                      IRepositoryQuery<chatGroups> chatGroupsQuery, 
                                      IRepositoryCommand<chatGroupMembers> chatGroupMembersCommand,
                                      IRepositoryQuery<InvEmployees> invEmployeesQuery, 
                                      IHubContext<NotificationHub> hub)
        {
            _iUserInformation = iUserInformation;
            _signalRQuery = signalRQuery;
            _chatGroupMembersQuery = chatGroupMembersQuery;
            _chatGroupsQuery = chatGroupsQuery;
            _chatGroupMembersCommand = chatGroupMembersCommand;
            _InvEmployeesQuery = invEmployeesQuery;
            _hub = hub;
        }
        public async Task<ResponseResult> Handle(addGroupMembersRequest request, CancellationToken cancellationToken)
        {
            //check is group exist
            var group = _chatGroupsQuery.TableNoTracking.Where(x => x.Id == request.groupId);
            if (!group.Any())
                return new ResponseResult()
                {
                    Note = "Group is not exist",
                    Result = Result.Failed
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            if (group.FirstOrDefault().groupCreatorId != userInfo.employeeId)
                return new ResponseResult()
                {
                    Note = "Only Group Admin can edit the group",
                    Result = Result.Failed
                };
            var employeesIds = request.employeesIds.Split(',').Select(x => int.Parse(x)).ToArray();
            var employees = _InvEmployeesQuery.TableNoTracking.Where(x => employeesIds.Contains(x.Id)).Select(x => x.Id).ToArray();
            var groupMembers = _chatGroupMembersQuery.TableNoTracking.Where(x => x.groupId == request.groupId).Select(x => x.memberId).ToArray();
            var empsToAdd = employees.Where(x => !groupMembers.Contains(x));
            var listOfChatGroupMembers = new List<chatGroupMembers>();
            foreach (var id in empsToAdd)
            {
                listOfChatGroupMembers.Add(new chatGroupMembers()
                {
                    groupId = request.groupId,
                    memberId = id,
                    isAdmin = false
                });
            }
            _chatGroupMembersCommand.AddRange(listOfChatGroupMembers);
            var saved = await _chatGroupMembersCommand.SaveAsync();
            if (saved)
            {
                var employeeConnectionId = _signalRQuery.TableNoTracking.Where(x => empsToAdd.Contains(x.InvEmployeesId) && x.isOnline == true).Select(x=> x.connectionId).ToArray();
                var signrResponse = new ResponseSignalR()
                {
                    groupId = request.groupId,
                    groupImg = group.FirstOrDefault().groupImage,
                    groupName = group.FirstOrDefault().groupName,
                };
                if(employeeConnectionId.Any())
                    await _hub.Clients.Groups(employeeConnectionId).SendAsync(defultData.addeddToChatGroup, signrResponse);

            }
            return new ResponseResult()
            {
                Note = saved ? "Success" : "Failed",
                Result = saved ? Result.Success : Result.Failed
            };
        }
        public class ResponseSignalR
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public string groupImg { get; set; }

        }
    }
}
