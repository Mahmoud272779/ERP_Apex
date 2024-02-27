using App.Infrastructure.Persistence.Context;
using App.Infrastructure.UserManagementDB;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount
{
    public class GetNotificationNotSeenCountHandler : IRequestHandler<GetNotificationNotSeenCountRequest, int>
    {
        private readonly IConfiguration _configuration;
        private readonly ClientSqlDbContext _clientcontext;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<App.Domain.Entities.Notification.NotificationSeen> _NotificationSeenQuery;



        public GetNotificationNotSeenCountHandler(IConfiguration configuration, ClientSqlDbContext clientcontext, IRepositoryQuery<Domain.Entities.Notification.NotificationSeen> notificationSeenQuery, iUserInformation iUserInformation)
        {
            _configuration = configuration;
            _clientcontext = clientcontext;
            _NotificationSeenQuery = notificationSeenQuery;
            _iUserInformation = iUserInformation;
        }

        public async Task<int> Handle(GetNotificationNotSeenCountRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            ERP_UsersManagerContext adminDB = new ERP_UsersManagerContext(_configuration);
            var databaseName = adminDB.UserApplications.Where(c => c.CompanyLogin == request.companyLogin).FirstOrDefault().DatabaseName;
            _clientcontext.Database.SetConnectionString(ConnectionString.connectionString(_configuration, databaseName));
            var _notificationsSeen = _NotificationSeenQuery.TableNoTracking.Where(c => c.UserId == userInfo.employeeId);
            var notificationsSeen = _notificationsSeen.Count();

            var broadCasts = adminDB.BroadCastMasters.AsNoTracking()
                              .Include(c => c.BroadCastCompanies)
                              .ThenInclude(c => c.Company)
                              .ToList()
                              .Where(c=> !_notificationsSeen.Where(x => x.isAdmin).Select(x => x.NotificationsMasterId).Contains(c.Id))
                              .Where(c => c.CDate < DateTime.Now.AddDays(30))
                              .Where(c => c.BroadCastCompanies.Any(x => x.Company.CompanyLogin == userInfo.companyLogin) || c.ForAll == true).Count();


            var notificationNotSeenCount = _clientcontext.NotificationsMaster
                .AsNoTracking()
                
                .Where(c => c.specialUserId == userInfo.employeeId || c.specialUserId == null)
                .Where(x => !_notificationsSeen.Where(c => c.NotificationsMasterId == x.Id).Any())
                .Count();

            notificationNotSeenCount += broadCasts;
            if (notificationNotSeenCount > 99)
                notificationNotSeenCount = 100;
            return notificationNotSeenCount;

        }
    }
}
