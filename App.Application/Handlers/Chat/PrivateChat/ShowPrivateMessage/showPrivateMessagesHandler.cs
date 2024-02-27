using App.Infrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Application.Handlers.Chat.PrivateChat.ShowPrivateMessage
{
    public class showPrivateMessagesHandler : IRequestHandler<showPrivateMessagesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvEmployees> _invEmployeesQuery;
        private readonly IRepositoryQuery<chatMessages> _chatMessagesQuery;
        private readonly IRepositoryCommand<chatMessages> _chatMessagesCommand;
        private readonly iUserInformation _iUserInformation;

        public showPrivateMessagesHandler(IRepositoryQuery<InvEmployees> invEmployeesQuery, IRepositoryQuery<chatMessages> chatMessagesQuery, IRepositoryCommand<chatMessages> chatMessagesCommand, iUserInformation iUserInformation)
        {
            _invEmployeesQuery = invEmployeesQuery;
            _chatMessagesQuery = chatMessagesQuery;
            _chatMessagesCommand = chatMessagesCommand;
            _iUserInformation = iUserInformation;
        }
        public async Task<ResponseResult> Handle(showPrivateMessagesRequest request, CancellationToken cancellationToken)
        {
            //Check if employee Exist 
            if(!_invEmployeesQuery.TableNoTracking.Where(x=> x.Id == request.messagesFromId).Any())
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.employeeNotExist,
                    ErrorMessageEn = ErrorMessagesEn.employeeNotExist
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            var messages = _chatMessagesQuery.TableNoTracking;
            var MyMessages = messages.Where(x => x.fromId == userInfo.employeeId && x.toId == request.messagesFromId);
            var HisMessages = messages.Where(x => x.fromId == request.messagesFromId && x.toId == userInfo.employeeId);
            var allMessages = MyMessages.Union(HisMessages)
                                        .OrderByDescending(x => x.date)
                                        .Skip((request.PageNumber - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .Select(x=> new {
                                                            x.Id,
                                                            x.date,
                                                            x.seen,
                                                            x.seenDate,
                                                            x.message,
                                                            isMine = x.fromId == userInfo.employeeId ? true : false,
                                        });

            var messagesToMakeSeen = HisMessages.Where(x => x.seen == false).ToList();
            messagesToMakeSeen.ForEach(x => x.seen = true);
            var updated = await _chatMessagesCommand.UpdateAsyn(messagesToMakeSeen);
            return new ResponseResult()
            {
                Note = allMessages.Any() ? "Success" : "NoDataFound",
                Data = allMessages,
                Result = allMessages.Any() ? Result.Success : Result.NoDataFound
            };
        }
    }
}
