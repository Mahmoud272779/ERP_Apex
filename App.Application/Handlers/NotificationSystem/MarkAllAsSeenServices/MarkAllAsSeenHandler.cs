using App.Domain.Entities.Notification;
using App.Infrastructure.UserManagementDB;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.MarkAllAsSeenServices
{

    public class MarkAllAsSeenHandler : IRequestHandler<MarkAllAsSeenRequest, ResponseResult>
    {
        private readonly IConfiguration _configuration;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<App.Domain.Entities.Notification.NotificationSeen> _NotificationSeenQuery;
        private readonly IRepositoryCommand<App.Domain.Entities.Notification.NotificationSeen> _NotificationSeenCommand;
        private readonly IRepositoryQuery<NotificationsMaster> _NotificationsMasterQuery;

        public MarkAllAsSeenHandler(IConfiguration configuration, iUserInformation iUserInformation, IRepositoryQuery<Domain.Entities.Notification.NotificationSeen> notificationSeenQuery, IRepositoryQuery<NotificationsMaster> notificationsMasterQuery, IRepositoryCommand<Domain.Entities.Notification.NotificationSeen> notificationSeenCommand)
        {
            _configuration = configuration;
            _iUserInformation = iUserInformation;
            _NotificationSeenQuery = notificationSeenQuery;
            _NotificationsMasterQuery = notificationsMasterQuery;
            _NotificationSeenCommand = notificationSeenCommand;
        }

        public async Task<ResponseResult> Handle(MarkAllAsSeenRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            ERP_UsersManagerContext adminDB = new ERP_UsersManagerContext(_configuration);
            var notificationsSeen = _NotificationSeenQuery.TableNoTracking;

            var broadCastSeend = notificationsSeen.Where(v => v.isAdmin && v.UserId == userInfo.userId).Select(v => v.NotificationsMasterId).ToArray();
            var broadCasts = adminDB.BroadCastMasters.AsNoTracking()
                             .Include(c => c.BroadCastCompanies)
                             .ThenInclude(c => c.Company)
                             .ToList()
                             .Where(c => c.CDate.Date > DateTime.Now.AddDays(-2).Date)
                             .Where(c => !broadCastSeend.Contains(c.Id))
                             .Where(c => c.BroadCastCompanies.Any(x => x.Company.CompanyLogin == userInfo.companyLogin) || c.ForAll == true)
                             .Select(c => new Domain.Entities.Notification.NotificationSeen
                             {
                                 isAdmin = true,
                                 NotificationsMasterId = c.Id,
                                 UserId = userInfo.employeeId
                             });



            var notifications = _NotificationsMasterQuery.TableNoTracking
                .Where(c => c.specialUserId == userInfo.employeeId || c.specialUserId == null)
                .Where(x => !notificationsSeen.Where(c => c.NotificationsMasterId == x.Id).Any())

                .Select(c => new Domain.Entities.Notification.NotificationSeen
                {
                    isAdmin = false,
                    NotificationsMasterId = c.Id,
                    UserId = userInfo.employeeId
                });
            if (notifications.Count() > 0 || broadCasts.Count() > 0)
            {
                _NotificationSeenCommand.AddRange(notifications);
                _NotificationSeenCommand.AddRange(broadCasts);
                var saved = await _NotificationSeenCommand.SaveAsync();
                return new ResponseResult
                {
                    Result = saved ? Result.Success : Result.Failed,
                };
            }
            else
                return new ResponseResult
                {
                    Result = Result.NoDataFound
                };

        }
    }
}
