using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Application.Handlers.Chat.Groups
{
    public class openGroupChatHandler : IRequestHandler<openGroupChatRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<chatGroups> _chatGroupsCommand;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly IRepositoryCommand<chatGroupMembers> _chatGroupMembersCommand;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IRepositoryQuery<chatMessages> _chatMessagesQuery;
        private readonly iUserInformation _iUserInformation;

        public openGroupChatHandler(IRepositoryCommand<chatGroups> chatGroupsCommand,
                                    IRepositoryQuery<chatGroups> chatGroupsQuery,
                                    IRepositoryCommand<chatGroupMembers> chatGroupMembersCommand,
                                    IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery,
                                    IRepositoryQuery<InvEmployees> invEmployeesQuery,
                                    iUserInformation iUserInformation,
                                    IRepositoryQuery<chatMessages> chatMessagesQuery)
        {
            _chatGroupsCommand = chatGroupsCommand;
            _chatGroupsQuery = chatGroupsQuery;
            _chatGroupMembersCommand = chatGroupMembersCommand;
            _chatGroupMembersQuery = chatGroupMembersQuery;
            _InvEmployeesQuery = invEmployeesQuery;
            _iUserInformation = iUserInformation;
            _chatMessagesQuery = chatMessagesQuery;
        }
        public async Task<ResponseResult> Handle(openGroupChatRequest request, CancellationToken cancellationToken)
        {
            //check group is exist 
            var groups = _chatGroupsQuery.TableNoTracking;
            if (!groups.Where(x => x.Id == request.GroupId).Any())
                return new ResponseResult()
                {
                    Note = "Group is not exist",
                    Result = Result.Failed
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            var messages = _chatMessagesQuery.TableNoTracking
                                             .Include(x=> x.InvEmployeesFrom)
                                             .Where(x => x.groupId == request.GroupId)
                                             .OrderByDescending(x => x.date)
                                             .Skip((request.PageNumber - 1) * request.PageSize)
                                             .Take(request.PageSize)
                                             .Select(x => new GroupMessages
                                             {
                                                 date = x.date.ToString(defultData.datetimeFormat),
                                                 message = x.message,
                                                 isMine = x.fromId == userInfo.employeeId ? true : false,
                                                 senderArabicName = x.InvEmployeesFrom.ArabicName,
                                                 senderLatinName = x.InvEmployeesFrom.LatinName,
                                                 senderImage = x.InvEmployeesFrom.ImagePath
                                             }).ToList();
            //var groupMember = _chatGroupMembersQuery.TableNoTracking
            //                                        .Include(x => x.invEmployeeMember)
            //                                        .Where(x => x.groupId == request.GroupId)
            //                                        .Select(x => new ChatMemebers
            //                                        {
            //                                            userId = x.memberId,
            //                                            arabicName = x.invEmployeeMember.ArabicName,
            //                                            latinName = x.invEmployeeMember.LatinName,
            //                                            image = x.invEmployeeMember.ImagePath
            //                                        }).ToList();
            var selectedGroup = groups.Where(x => x.Id == request.GroupId).FirstOrDefault();
            var response = new GroupChatResponse()
            {
                //members = groupMember,
                messages = messages,
                groupId = request.GroupId,
                groupName = selectedGroup.groupName,
                image = selectedGroup.groupImage,
                canEdit = userInfo.employeeId == selectedGroup.groupCreatorId ? true : false,

            };
            return new ResponseResult()
            {
                Note = messages.Any() ? "" : "no messages found",
                Data = response,
                Result = messages.Any() ? Result.Success : Result.NoDataFound
            };
        }
    }

    public class GroupChatResponse
    {
        //public List<ChatMemebers> members { get; set; }
        public List<GroupMessages> messages { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string image { get; set; }
        public bool canEdit { get; set; }

    }
    public class ChatMemebers
    {
        public int userId { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string image { get; set; }
    }
    public class GroupMessages
    {
        public string message { get; set; }
        public string date { get; set; }
        public bool isMine { get; set; }
        public string senderArabicName { get; set; }
        public string senderLatinName { get; set; }
        public string senderImage { get; set; }
    }
}
