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
    public class removeGroupMemberHandler : IRequestHandler<removeGroupMemberRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryCommand<chatGroupMembers> _chatGroupMembersCommand;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IHubContext<NotificationHub> _hub;
        public removeGroupMemberHandler(iUserInformation iUserInformation, IRepositoryQuery<signalR> signalRQuery, IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery, IRepositoryCommand<chatGroupMembers> chatGroupMembersCommand, IRepositoryQuery<chatGroups> chatGroupsQuery, IRepositoryQuery<InvEmployees> invEmployeesQuery, IHubContext<NotificationHub> hub)
        {
            _iUserInformation = iUserInformation;
            _signalRQuery = signalRQuery;
            _chatGroupMembersQuery = chatGroupMembersQuery;
            _chatGroupMembersCommand = chatGroupMembersCommand;
            _chatGroupsQuery = chatGroupsQuery;
            _InvEmployeesQuery = invEmployeesQuery;
            _hub = hub;
        }
        public async Task<ResponseResult> Handle(removeGroupMemberRequest request, CancellationToken cancellationToken)
        {
            //check is group exist
            var group = _chatGroupsQuery.TableNoTracking.Where(x => x.Id == request.groupId);
            if (!group.Any())
                return new ResponseResult() { Note = "group is not exist" ,Result = Result.Failed};
            //check is employee exist 
            var employee = _InvEmployeesQuery.TableNoTracking.Where(x => x.Id == request.employeeId);
            if(!employee.Any())
                return new ResponseResult() { Note = "employee is not exist", Result = Result.Failed };
            //check if employee exist in group 
            var groupEmployee = _chatGroupMembersQuery.TableNoTracking.Where(x => x.groupId == request.groupId && x.memberId == request.employeeId);
            if(!groupEmployee.Any())
                return new ResponseResult() { Note = "employee is not exist in this group", Result = Result.Failed };
            //remove employee from goroup
            var userInfo = await _iUserInformation.GetUserInformation();
            //check is the user is the admin of the group
            if(group.FirstOrDefault().groupCreatorId != userInfo.employeeId)
                return new ResponseResult() { Note = "only admin of the group can remove members", Result = Result.Failed };
            _chatGroupMembersCommand.RemoveRange(groupEmployee);
            var saved = await _chatGroupMembersCommand.SaveAsync();
            if (saved)
            {
                var memberConnectionID = _signalRQuery.TableNoTracking.Where(x=> x.InvEmployeesId == request.employeeId).FirstOrDefault();
                if(memberConnectionID != null)
                {
                    if (memberConnectionID.isOnline)
                    {
                        var response = new responseSignalR()
                        {
                            NoteEn = $"The Admin of The Group has Kicked you out chat group {group.FirstOrDefault().groupName}",
                            NoteAr = $"تم خروجك من مجموعه الدردشه {group.FirstOrDefault().groupName}",
                            groupId = request.groupId,
                        };
                        await _hub.Clients.Client(memberConnectionID.connectionId).SendAsync(defultData.removedFromGroup,response);
                    }
                }
            }
            return new ResponseResult()
            {
                Note = saved ? "Remove success" : "Remove Faild",
                Result = saved ? Result.Success : Result.Failed
            };
        }
        public class responseSignalR
        {
            public string NoteAr { get; set; }
            public string NoteEn { get; set; }
            public int groupId { get; set; }
        }
    }
}
