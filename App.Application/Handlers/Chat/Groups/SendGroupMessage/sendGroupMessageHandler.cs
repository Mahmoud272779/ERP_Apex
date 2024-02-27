using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Application.Handlers.Chat.Groups.SendGroupMessage
{
    public class sendGroupMessageHandler : IRequestHandler<sendGroupMessageRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryCommand<chatMessages> _chatMessagesCommand;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IRepositoryQuery<signalR> _SignalRQuery;
        private readonly IMemoryCache _memoryCache;

        public sendGroupMessageHandler(iUserInformation iUserInformation,
                                       IHubContext<NotificationHub> hub,
                                       IMemoryCache memoryCache,
                                       IRepositoryQuery<chatGroups> chatGroupsQuery,
                                       IRepositoryCommand<chatMessages> chatMessagesCommand,
                                       IRepositoryQuery<signalR> signalRQuery,
                                       IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery)
        {
            _iUserInformation = iUserInformation;
            _hub = hub;
            _memoryCache = memoryCache;
            _chatGroupsQuery = chatGroupsQuery;
            _chatMessagesCommand = chatMessagesCommand;
            _SignalRQuery = signalRQuery;
            _chatGroupMembersQuery = chatGroupMembersQuery;
        }
        public async Task<ResponseResult> Handle(sendGroupMessageRequest request, CancellationToken cancellationToken)
        {
            //Check is group exist
            var group = _chatGroupsQuery.TableNoTracking.Where(x => x.Id == request.groupId);
            if (!group.Any())
                return new ResponseResult()
                {
                    Note = "Group is not exist",
                    Result = Result.Failed
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            //save message in database 
            var messageSaved = await _chatMessagesCommand.AddAsync(new chatMessages()
            {
                message = request.message,
                date = DateTime.Now,
                groupId = request.groupId,
                fromId = userInfo.employeeId,
                isPrivate = false
            });
            var signalMessage = new ResponseSignalR()
            {
                groupId = request.groupId,
                message = request.message,
                senderArabicName = userInfo.employeeNameAr.ToString(),
                senderLatinName = userInfo.employeeNameEn.ToString(),
                senderImage = userInfo.employeeImg,
                senderId = userInfo.employeeId
            };
            var groupMembers = _chatGroupMembersQuery.TableNoTracking.Where(x=> x.groupId == request.groupId).Select(x=> x.memberId).ToArray();
            var usersConnectionID = _SignalRQuery.TableNoTracking.Where(x => x.isOnline == true).Where(x=> groupMembers.Contains(x.InvEmployeesId)).Select(x => x.connectionId).ToList();
            if (usersConnectionID.Any())
            {
                await _hub.Clients.Groups(usersConnectionID).SendAsync(defultData.groupMessage, signalMessage);
            }
            return new ResponseResult()
            {
                Note = "Success",
                Result = Result.Success
            };
        }

    }
    public class ResponseSignalR
    {
        public int groupId { get; set; }
        public string message { get; set; }
        public int senderId { get; set; }
        public string senderArabicName { get; set; }
        public string senderLatinName { get; set; }
        public string senderImage { get; set; }
    }
}
