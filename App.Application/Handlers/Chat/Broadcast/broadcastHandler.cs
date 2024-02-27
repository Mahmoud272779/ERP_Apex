using App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount;
using App.Application.Handlers.NotificationSystem.insertBroadCastIntoDatabase;
using App.Application.SignalRHub;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using MediatR;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Broadcast
{
    public class broadcastHandler : IRequestHandler<broadcastRequest, ResponseResult>
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMemoryCache _memoryCache;
        private ERP_UsersManagerContext _ERP_UsersManagerContext;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly ClientSqlDbContext _ClientSqlDbContext;
        public broadcastHandler(IHubContext<NotificationHub> hub, IMemoryCache memoryCache, IConfiguration configuration, IMediator mediator, ClientSqlDbContext clientSqlDbContext)
        {
            _hub = hub;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _mediator = mediator;
            _ClientSqlDbContext = clientSqlDbContext;
        }
        public async Task<ResponseResult> Handle(broadcastRequest request, CancellationToken cancellationToken)
        {


            if (request.SecurityKey != defultData.userManagmentApplicationSecurityKey)
                return new ResponseResult()
                {
                    Note = "Security Key is Wrong",
                    Result = Result.Failed
                };


            if (request.AllCompanies)
            {
                await _hub.Clients.All.SendAsync(defultData.Notification, new ResponseSignalR
                {
                    title = request.title,
                    desc = request.desc,
                });
                return new ResponseResult()
                {
                    Note = "Broadcast Sent successful",
                    Result = Result.Success
                };
            }
            _ERP_UsersManagerContext = new ERP_UsersManagerContext(_configuration);

            var companies = !request.AllCompanies ? request.companiesId.Split(',').ToArray() : null;
            var companiesTable = _ERP_UsersManagerContext.UserApplications
                                .Where(c=> !string.IsNullOrEmpty(c.DatabaseName))
                                .Where(c => !request.AllCompanies ? companies.Contains(c.Id.ToString()) : true).ToHashSet();

            if (companiesTable.Any())
            {
                foreach (var item in companiesTable)
                {
                    try
                    {
                        _ClientSqlDbContext.Database.SetConnectionString(ConnectionString.connectionString(_configuration, item.DatabaseName));
                        try
                        {
                            _ClientSqlDbContext.Database.OpenConnection();
                        }
                        catch (Exception)
                        {

                            continue;
                        }

                        var signalRConnections = _ClientSqlDbContext.signalR.Where(c => c.isOnline).GroupBy(c => c.InvEmployeesId).Select(c => c.OrderBy(c=> c.Id).LastOrDefault());
                        if (signalRConnections.Any())
                        {
                            foreach (var SignalRItem in signalRConnections)
                            {
                                await _hub.Clients.Clients(SignalRItem.connectionId).SendAsync(defultData.Notification, new ResponseSignalR
                                {
                                    title = request.title,
                                    desc = request.desc,
                                });
                            }
                        }
                        _ClientSqlDbContext.Database.CloseConnection();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return new ResponseResult()
            {
                Note = "Broadcast Sent successful",
                Result = Result.Success
            };

        }
    }

}
