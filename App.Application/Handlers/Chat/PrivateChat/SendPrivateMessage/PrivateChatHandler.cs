using App.Application.Handlers.PrivateChat.Chat;
using App.Application.Helpers;
using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using App.Domain.Models.Response.ChatResponse;
using App.Infrastructure;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
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

namespace App.Application.Handlers.Chat.PrivateChat
{
    public class PrivateChatHandler : IRequestHandler<PrivateChatRequest, ResponseResult>
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly iUserInformation _iUserInformation;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryQuery<InvEmployees> _invEmployeesQuery;
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryCommand<chatMessages> _chatMessagesCommand;

        public PrivateChatHandler(IHubContext<NotificationHub> hub,
                                  iUserInformation iUserInformation,
                                  IMemoryCache memoryCache,
                                  IHttpContextAccessor httpContext,
                                  IRepositoryQuery<InvEmployees> invEmployeesQuery,
                                  IRepositoryQuery<signalR> signalRQuery,
                                  IRepositoryCommand<chatMessages> chatMessagesCommand)
        {
            _hub = hub;
            _iUserInformation = iUserInformation;
            _memoryCache = memoryCache;
            _httpContext = httpContext;
            _invEmployeesQuery = invEmployeesQuery;
            _signalRQuery = signalRQuery;
            _chatMessagesCommand = chatMessagesCommand;
        }
        public async Task<ResponseResult> Handle(PrivateChatRequest request, CancellationToken cancellationToken)
        {
            ChatSignalRResponse HubResponse;
            var userInfo = await _iUserInformation.GetUserInformation();
            var Message = new ChatSignalRResponse()
            {
                date = DateTime.Now.ToString(defultData.datetimeFormat),
                IdFrom = userInfo.employeeId,
                imgFrom = userInfo.employeeImg,
                message = request.message
            };
            var employees = _invEmployeesQuery.TableNoTracking.Where(x=> x.Id == request.IdTo);
            if(!employees.Any()) 
            {
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = "User Is Not Exist"
                };
            }
            var UserToSignalRConnection = _signalRQuery.TableNoTracking.Where(x=> x.InvEmployeesId == request.IdTo && x.isOnline);
            string SignalRConnectionId = string.Empty;
            if (UserToSignalRConnection.Any())
                SignalRConnectionId = UserToSignalRConnection.FirstOrDefault().connectionId;
            HubResponse = new ChatSignalRResponse()
            {
                date = DateTime.Now.ToString(defultData.datetimeFormat),
                IdFrom = userInfo.employeeId,
                imgFrom = userInfo.employeeImg,
                arabicName = userInfo.employeeNameAr.ToString(),
                latinName = userInfo.employeeNameEn.ToString(),
                message= request.message,
            };
            // Set Save Message here
            _chatMessagesCommand.Add(new chatMessages()
            {
                fromId = userInfo.employeeId,
                toId = request.IdTo,
                date = DateTime.Now,
                seen = false,
                isPrivate = true,
                message = request.message
            });
            await _chatMessagesCommand.SaveAsync();
            // Set Save Message here
            if (SignalRConnectionId == string.Empty)
            {
                return new ResponseResult()
                {

                    Result = Result.Success,
                    Note = "Message Sent Success And User Is Offline"
                };
            }
            await _hub.Clients.Clients(SignalRConnectionId).SendAsync(defultData.privateChat, HubResponse);
            return new ResponseResult()
            {
                Result = Result.Success,
                Note = "Message Sent Success"
            };
        }
    }
}
