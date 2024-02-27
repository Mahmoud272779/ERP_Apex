using App.Application.Services.HelperService.SecurityIntegrationServices;
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

namespace App.Application.Handlers.Chat.Groups
{
    public class createGroupHandler : IRequestHandler<createGroupRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryCommand<chatGroups> _chatGroupsCommand;
        private readonly IRepositoryQuery<chatGroups> _chatGroupsQuery;
        private readonly IRepositoryCommand<chatGroupMembers> _chatGroupMembersCommand;
        private readonly IRepositoryQuery<chatGroupMembers> _chatGroupMembersQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IMemoryCache _memoryCache;
        private readonly iUserInformation _iUserInformation;
        private readonly ISecurityIntegrationService _ISecurityIntegrationService;
        private readonly IHubContext<NotificationHub> _hub;
        public createGroupHandler(IRepositoryCommand<chatGroups> chatGroupsCommand,
                                  IRepositoryQuery<chatGroups> chatGroupsQuery,
                                  IRepositoryQuery<InvEmployees> invEmployeesQuery,
                                  iUserInformation iUserInformation,
                                  IRepositoryCommand<chatGroupMembers> chatGroupMembersCommand,
                                  IRepositoryQuery<chatGroupMembers> chatGroupMembersQuery,
                                  ISecurityIntegrationService iSecurityIntegrationService,
                                  IHubContext<NotificationHub> hub,
                                  IRepositoryQuery<signalR> signalRQuery)
        {
            _chatGroupsCommand = chatGroupsCommand;
            _chatGroupsQuery = chatGroupsQuery;
            _InvEmployeesQuery = invEmployeesQuery;
            _iUserInformation = iUserInformation;
            _chatGroupMembersCommand = chatGroupMembersCommand;
            _chatGroupMembersQuery = chatGroupMembersQuery;
            _ISecurityIntegrationService = iSecurityIntegrationService;
            _hub = hub;
            _signalRQuery = signalRQuery;
        }
        public async Task<ResponseResult> Handle(createGroupRequest request, CancellationToken cancellationToken)
        {
            //check if group name exist
            var groups = _chatGroupsQuery.TableNoTracking;
            if (groups.Where(x => x.groupName == request.groupName).Any())
                return new ResponseResult()
                {
                    Note = "Group name is Exist",
                    Result = Result.Failed
                };
            var employeesIds = request.groupMemebers.Split(',').Select(x => int.Parse(x)).ToArray();
            var userInfo = await _iUserInformation.GetUserInformation();
            //check employees
            var group = new chatGroups()
            {
                allowReply = request.allowReply,
                canExit = request.canExit,
                groupCreatorId = userInfo.employeeId,
                groupName = request.groupName,
                creationDate = DateTime.Now,
                isEnded = false
            };
            
            var groupAdded = await _chatGroupsCommand.AddAsync(group);
            if (groupAdded)
            {
                var employeesIdsExists = _InvEmployeesQuery.TableNoTracking.Where(x => employeesIds.Contains(x.Id)).Select(x => x.Id);
                var listOfMembers = new List<chatGroupMembers>();
                listOfMembers.Add(new chatGroupMembers
                {
                    groupId = group.Id,
                    isAdmin = true,
                    memberId = userInfo.employeeId
                });
                foreach (var employee in employeesIdsExists)
                {
                    if (employee == userInfo.employeeId)
                        continue;
                    listOfMembers.Add(new chatGroupMembers
                    {
                        groupId = group.Id,
                        isAdmin = false,
                        memberId = employee
                    });
                }
                _chatGroupMembersCommand.AddRange(listOfMembers);
                var membersSaved = await _chatGroupMembersCommand.SaveAsync();
                if (membersSaved)
                {
                    var companyInfo = await _ISecurityIntegrationService.getCompanyInformation();
                    var onlineMembers = _signalRQuery.TableNoTracking.Where(x => employeesIdsExists.Contains(x.InvEmployeesId) && x.isOnline).Select(x=> x.connectionId).ToList();
                    if (onlineMembers.Any())
                    {
                        await _hub.Clients.Clients(onlineMembers).SendAsync(defultData.joinGroup, new signalRGroupJoin
                        {
                            groupId = group.Id,
                            groupImage = group.groupImage,
                            groupName = group.groupName,
                        });
                    }
                        return new ResponseResult()
                        {
                            Note = "creation Success",
                            Result = Result.Success
                        };
                }
                else
                {
                    _chatGroupsCommand.Remove(group);
                    await _chatGroupsCommand.SaveAsync();
                    return new ResponseResult()
                    {
                        Note = "creation Failed",
                        Result = Result.Failed
                    };
                }
            }
            else
            {
                return new ResponseResult()
                {
                    Note = "Error 500",
                    Result = Result.Failed
                };
            }
            return new ResponseResult()
            {
                Note = "creation Failed",
                Result = Result.Failed
            };
        }

        public class signalRGroupJoin
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public string groupImage { get; set; }
        }
    }
}
