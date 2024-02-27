using App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount;
using App.Application.SignalRHub;
using App.Domain.Entities.Notification;
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

namespace App.Application.Handlers.NotificationSystem.InsertNotification
{
    public class InsertNotificationHandler : IRequestHandler<InsertNotificationRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<NotificationsMaster> _NotificationsMasterCommand;
        private readonly IRepositoryQuery<signalR> _signalRQuer;
        private readonly IHubContext<NotificationHub> _hubContextl;
        private readonly IMediator _mediator;
        private readonly iUserInformation _userInformation;

        public InsertNotificationHandler(IRepositoryCommand<NotificationsMaster> notificationsMasterQuery, IHubContext<NotificationHub> hubContextl, IMediator mediator, iUserInformation userInformation, IRepositoryQuery<signalR> signalRQuer)
        {
            _NotificationsMasterCommand = notificationsMasterQuery;
            _hubContextl = hubContextl;
            _mediator = mediator;
            _userInformation = userInformation;
            _signalRQuer = signalRQuer;
        }
        public async Task<ResponseResult> Handle(InsertNotificationRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _userInformation.GetUserInformation();
            var ele = new NotificationsMaster
            {
                cDate = DateTime.Now,
                isSystem = request.isSystem,
                Desc = request.Desc,
                title = request.title,
                pageId = request.pageId,
                specialUserId = request.specialUserId,
                insertedById = userInfo.employeeId,
                routeUrl = request.routeUrl,
                titleEn = request.titleEn,
                DescEn = request.DescEn
            };
            var inserted = await _NotificationsMasterCommand.AddAsync(ele);
            var signalRConnectionsIds = _signalRQuer.TableNoTracking.Where(c=> c.isOnline).ToList();
            if (signalRConnectionsIds.Any())
            {
                var notificationsCount = await _mediator.Send(new GetNotificationNotSeenCountRequest { companyLogin = userInfo.companyLogin, employeeId = userInfo.employeeId });
                var HubResponse = new ResponseSignalR
                {
                    desc = request.Desc,
                    title = request.title,
                    descEn = request.DescEn,
                    titleEn = request.DescEn,
                    notificationNotSeenCount = notificationsCount,
                    RouteURL = "",
                    Id = ele.Id,
                    notificationTypeId = request.notificationTypeId,
                    date = ele.cDate.ToString(defultData.datetimeFormat),
                    hasRedirection= request.hasRedirection,
                };
                if (request.specialUserId != null)
                {
                    var userConnectionsId = signalRConnectionsIds.Where(c => c.InvEmployeesId == request.specialUserId).FirstOrDefault().connectionId;
                    await _hubContextl.Clients.Client(userConnectionsId).SendAsync(defultData.Notification, HubResponse);
                }
                else
                {
                    await _hubContextl.Clients.Clients(signalRConnectionsIds.Select(c=> c.connectionId)).SendAsync(defultData.Notification, HubResponse);
                }
            }

            return new ResponseResult
            {
                Result = inserted ? Result.Success : Result.Failed
            };
        }
    }
}
