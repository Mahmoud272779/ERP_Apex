using App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount;
using App.Domain.Entities.Notification;
using App.Domain.Entities.Process;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.GetUserNotifications
{
    public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsRequest, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryQuery<NotificationsMaster> _NotificationsMasterQuery;
        private readonly IRepositoryQuery<App.Domain.Entities.Notification.NotificationSeen> _NotificationSeenQuery;
        private readonly iUserInformation iUserInformation;
        private readonly IConfiguration _configuration;

        public GetUserNotificationsHandler(IMediator mediator, IRepositoryQuery<NotificationsMaster> notificationsMasterQuery, IConfiguration configuration, IRepositoryQuery<App.Domain.Entities.Notification.NotificationSeen> notificationSeenQuery, iUserInformation iUserInformation)
        {
            _mediator = mediator;
            _NotificationsMasterQuery = notificationsMasterQuery;
            _configuration = configuration;
            _NotificationSeenQuery = notificationSeenQuery;
            this.iUserInformation = iUserInformation;
        }
        public async Task<ResponseResult> Handle(GetUserNotificationsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await iUserInformation.GetUserInformation();
            //var notificationNotSeenCount = await _mediator.Send(new GetNotificationNotSeenCountRequest { companyLogin = userInfo.companyLogin, employeeId = userInfo.employeeId });
            int notificationNotSeenCount = 0;
            var notifications = _NotificationsMasterQuery
                                .TableNoTracking
                                .Where(c => c.cDate < DateTime.Now.AddDays(5))
                                .Where(c => c.specialUserId == null || c.specialUserId == userInfo.employeeId)
                                .ToList();
            ERP_UsersManagerContext adminDB = new ERP_UsersManagerContext(_configuration);

            var notificationsSeen = _NotificationSeenQuery.TableNoTracking.Where(c => c.UserId == userInfo.employeeId);
           
            var res = notifications.Select(c => new GetUserNotificationsResponseObject
                                {
                                    Id = c.Id,
                                    title = c.title,
                                    desc = c.Desc,
                                    titleEn = c.titleEn,
                                    descEn = c.DescEn,
                                    hasRedirection = string.IsNullOrEmpty(c.pageId.ToString()),
                                    routeURL = c.routeUrl,
                                    seen = notificationsSeen.Where(x=> x.NotificationsMasterId == c.Id).Any(),
                                    notificationType = (int)notificationTypes.System,
                                    date = c.cDate.ToString(defultData.datetimeFormat)
                                }).ToList();
            notificationNotSeenCount += res.Where(c => c.seen == false).Count();

            var broadCasts = new List<GetUserNotificationsResponseObject>();
            try
            {
                broadCasts = adminDB.BroadCastMasters.AsNoTracking()
                                                   .Include(c => c.BroadCastCompanies)
                                                   .ThenInclude(c => c.Company)
                                                   .ToList()
                                                   .Where(c => c.CDate.Date > DateTime.Now.AddDays(-2).Date)
                                                   .Where(c => c.BroadCastCompanies.Any(x => x.Company.CompanyLogin == userInfo.companyLogin) || c.ForAll == true)
                                                   .Select(c => new GetUserNotificationsResponseObject
                                                   {
                                                       Id = c.Id,
                                                       title = c.Title,
                                                       desc = c.Body,
                                                       titleEn = c.Title,
                                                       descEn = c.Body,
                                                       routeURL = "",
                                                       seen = notificationsSeen.Where(d => d.isAdmin).Any(d => d.UserId == userInfo.employeeId && d.NotificationsMasterId ==c.Id),
                                                       notificationType = (int)notificationTypes.admin,
                                                       date = c.CDate.ToString(defultData.datetimeFormat),
                                                       hasRedirection = false,
                                                   })
                                                   .ToList();
                notificationNotSeenCount += broadCasts.Where(c => c.seen == false).Count();

            }
            catch (Exception)
            {

               
            }
                


            var allNotifications = res.Union(broadCasts);
            allNotifications = allNotifications.OrderByDescending(x => x.date); 
            var count = allNotifications.Count();
            var pagin = allNotifications.Skip(((request.PageNumber ?? 1) - 1) * request.PageSize??20).Take(request.PageSize??20).ToList();

            GetUserNotificationsResponseMasterObject getUserNotificationsResponseMasterObject = new GetUserNotificationsResponseMasterObject
            {
                notificationNotSeenCount = notificationNotSeenCount,
                Items = pagin
            };
            double MaxPageNumber = notifications.ToList().Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new ResponseResult
            {
                Data = getUserNotificationsResponseMasterObject,
                DataCount = res.Count(),
                TotalCount = count,
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : ""),
                Result = Result.Success
            };
        }
    }
}
