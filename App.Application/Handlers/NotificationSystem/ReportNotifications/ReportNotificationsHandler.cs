using App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount;
using App.Application.Handlers.NotificationSystem.InsertNotification;
using App.Application.Handlers.PrivateChat.Chat;
using App.Application.SignalRHub;
using App.Domain.Entities;
using App.Infrastructure;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Math;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.ReportNotifications
{
    public class ReportNotificationsHandler : IRequestHandler<ReportNotificationsRequest, bool>
    {
        private readonly IMediator _mediator;
        private readonly ClientSqlDbContext _ClientSqlDbContext;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hub;
        public ReportNotificationsHandler(IRepositoryQuery<rules> rulesQuery, IMediator mediator, ClientSqlDbContext clientSqlDbContext, IConfiguration configuration, IHubContext<NotificationHub> hub)
        {
            _mediator = mediator;
            _ClientSqlDbContext = clientSqlDbContext;
            _configuration = configuration;
            _hub = hub;
        }
        public async Task<bool> Handle(ReportNotificationsRequest request, CancellationToken cancellationToken)
        {
            Thread.Sleep(1000);


            var notificationNotSeenCount = await _mediator.Send(new GetNotificationNotSeenCountRequest { companyLogin = StringEncryption.DecryptString(contextHelper.CompanyLogin(request.token)), employeeId = int.Parse(StringEncryption.DecryptString(contextHelper.GetEmployeeId(request.token))) });
            var dbConnectionstring = ConnectionString.connectionString(_configuration, StringEncryption.DecryptString(contextHelper.GetDBName(request.token)));

            _ClientSqlDbContext.Database.SetConnectionString(dbConnectionstring);

            var userinfo = _ClientSqlDbContext.userAccount.AsNoTracking().Include(c => c.UserAndPermission).Include(c => c.otherSettings).ThenInclude(c => c.OtherSettingsStores).Where(c => c.employeesId.ToString() == StringEncryption.DecryptString(contextHelper.GetEmployeeId(request.token))).FirstOrDefault();

            var generalSettings = _ClientSqlDbContext.invGeneralSettings.FirstOrDefault();
            var allStores = _ClientSqlDbContext.stores.AsNoTracking();
            var userRules = _ClientSqlDbContext.rules.AsNoTracking().Where(c => c.permissionListId == userinfo.UserAndPermission.FirstOrDefault().permissionListId);
            var invoices = _ClientSqlDbContext.invoiceDetails.AsNoTracking().Include(x => x.InvoicesMaster)
                                                                           .Include(x => x.Items)
                                                                           .Include(x => x.Items.Stores)
                                                                           .Include(x => x.Items.Units);
            var signalrArr = new List<ResponseSignalR>();
            //Demand Limit Report
            var notificationObject_DemandLimit = new ResponseSignalR
            {
                title = "تنبيه حد الطلب",
                desc = "",
                titleEn = "Demand Limit",
                descEn = "",
                date = DateTime.Now.ToString(defultData.datetimeFormat),
                hasRedirection = true,
                notificationTypeId = (int)Enums.notificationTypes.System,
                RouteURL = "repository/report/demand-limit",
                Id = -1,
                notificationNotSeenCount = notificationNotSeenCount
            };
            bool have_notificationObject_DemandLimit = false;


            var notificationObject_ExpiredItems = new ResponseSignalR
            {
                title = "صلاحية اصناف",
                desc = "",
                titleEn = "Expired Items",
                descEn = "",
                date = DateTime.Now.ToString(defultData.datetimeFormat),
                hasRedirection = true,
                notificationTypeId = (int)Enums.notificationTypes.System,
                RouteURL = "repository/report/Expiration-Dates",
                Id = -2,
                notificationNotSeenCount = notificationNotSeenCount
            };
            bool have_notificationObject_ExpiredItems = false;
            foreach (var storeId in userinfo.otherSettings.FirstOrDefault().OtherSettingsStores.Select(c => c.InvStpStoresId))
            {
                var currentStore = allStores.Where(c => c.Id == storeId).FirstOrDefault();
                #region Demand Limit Report
                if (userRules.Where(c => c.subFormCode == (int)SubFormsIds.DemandLimit).FirstOrDefault().isShow && generalSettings.Other_DemandLimitNotification)
                {
                    var checkStoreItems = invoices
                                                   .Where(c => !c.InvoicesMaster.IsDeleted)
                                                   .Where(x => x.InvoicesMaster.StoreId == storeId)
                                                   .Where(c => c.Items.Stores.Any())
                                                   .Where(x => x.Items.Stores.Where(c => c.StoreId == storeId).First().DemandLimit != 0)
                                                   .ToList()
                                                   .GroupBy(c => new { c.ItemId })
                                                   .ToList()
                                                   .Where(x => x.First().Items.Stores.Where(c => c.StoreId == storeId).FirstOrDefault()?.DemandLimit>=
                                                   Convert.ToDecimal(ReportData<InvoiceDetails>.Quantity(x, 1)))
                                                   .Select(c => c.FirstOrDefault());
                    if (checkStoreItems.Any())
                    {
                        notificationObject_DemandLimit.desc += $"{currentStore.ArabicName} عدد اصناف {checkStoreItems.Count()}    <br>";
                        notificationObject_DemandLimit.descEn += $"{currentStore.LatinName} Items Count {checkStoreItems.Count()} <br>";
                        have_notificationObject_DemandLimit = true;
                    }
                    #endregion
                }
                if (userRules.Where(c => c.subFormCode == (int)SubFormsIds.GetDetailsOfExpiredItems).FirstOrDefault().isShow && generalSettings.Other_ExpireNotificationFlag)
                {
                    #region Expir Items 
                    var ExpiredItems = invoices.Where(x => !x.InvoicesMaster.IsDeleted)
                                                        .Where(a => a.ItemTypeId == (int)ItemTypes.Expiary)
                                                        .Where(a => a.InvoicesMaster.StoreId == storeId)
                                                        .Where(a => a.ExpireDate.Value.Date > DateTime.Now.Date && a.ExpireDate.Value.Date <= DateTime.Now.AddDays(generalSettings.Other_ExpireNotificationValue).Date)
                                                        .ToList()
                                                        .GroupBy(x => new { x.ExpireDate });
                    if (ExpiredItems.Any())
                    {
                        notificationObject_ExpiredItems.desc += $"{currentStore.ArabicName} عدد اصناف {ExpiredItems.Count()}    <br>";
                        notificationObject_ExpiredItems.descEn += $"{currentStore.LatinName} items count {ExpiredItems.Count()} <br>";
                        have_notificationObject_ExpiredItems = true;
                    }
                }

                #endregion



            }
            if (have_notificationObject_DemandLimit)
            {
                signalrArr.Add(notificationObject_DemandLimit);
                notificationNotSeenCount++;
            }
            if (have_notificationObject_ExpiredItems)
            {
                signalrArr.Add(notificationObject_ExpiredItems);
                notificationNotSeenCount++;
            }
            signalrArr.ForEach(c => c.notificationNotSeenCount = notificationNotSeenCount);
            SendNotification(signalrArr, request.connectionId);
            _ClientSqlDbContext.Dispose();
            return true;
        }
        private async void SendNotification(List<ResponseSignalR> obj, string connectionId)
        {

            foreach (var item in obj)
            {
                await _hub.Clients.Clients(connectionId).SendAsync(defultData.Notification, new ResponseSignalR
                {
                    title = item.title,
                    desc = item.desc,
                    titleEn = item.titleEn,
                    descEn = item.descEn,   
                    date = item.date,
                    notificationNotSeenCount = obj.Count(),
                    hasRedirection = item.hasRedirection,
                    Id = item.Id,
                    notificationTypeId = item.notificationTypeId,
                    RouteURL = item.RouteURL
                });
            }
        }

    }
}
