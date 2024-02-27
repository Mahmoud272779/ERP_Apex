using App.Application.SignalRHub;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.DemandLimitNotificationSystem
{
    public class DemandLimitNotification : IDemandLimitNotificationService
    {
        private readonly ClientSqlDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IConfiguration _configuration;
        public DemandLimitNotification(ClientSqlDbContext dbContext, 
                                       IMemoryCache memoryCache, 
                                       IConfiguration configuration)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        public async Task DemandLimitNotificationServ()
        {
            var clients = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            if (clients.Any())
            {
                var companies = clients.GroupBy(x => x.CompanyLogin);
                foreach (var item in companies)
                {
                    var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                       $"Initial Catalog={item.First().DBName};" +
                                       $"user id={_configuration["ApplicationSetting:UID"]};" +
                                       $"password={_configuration["ApplicationSetting:Password"]};" +
                                       $"MultipleActiveResultSets=true;";
                    _dbContext.Database.SetConnectionString(connectionString);
                    var GetAllStores = _dbContext.stores;
                    foreach (var user in item)
                    {
                        var stores = _dbContext.otherSettingsStores.AsNoTracking()
                                                                   .Include(x => x.otherSettings)
                                                                   .Include(x => x.otherSettings.userAccount)
                                                                   .Include(x => x.otherSettings.userAccount.employees)
                                                                   .Where(x => x.otherSettings.userAccount.employees.Id == user.EmployeeId)
                                                                   .Select(c=> c.InvStpStoresId)
                                                                   .ToArray();
                        foreach (var store in stores)
                        {
                            var findItems = _dbContext.invoiceDetails.AsNoTracking()
                                                                    .Include(x => x.InvoicesMaster)
                                                                    .Include(x => x.Items)
                                                                    .Include(x => x.Items.Stores)
                                                                    .Where(x => x.InvoicesMaster.StoreId == store)
                                                                    .Where(x => x.Items.Stores.Where(c => c.StoreId == store).First().DemandLimit != 0).Any();
                            var signalrResponse = new ResponseSignalRDemandLimitNotification()
                            {
                                arabicNote = $"يوجد تنبية بحد الطلب في مخزن {GetAllStores.Where(c=> c.Id == store).Select(c=> c.ArabicName)}",
                                latinNote = $"There is notification of Demand Limit for store {GetAllStores.Where(c=> c.Id == store).Select(c=> c.ArabicName)}"
                            };
                            if (findItems)
                            {
                                await _hub.Clients.Groups(clients.Where(c => c.CompanyLogin == item.First().CompanyLogin).Select(c => c.connectionId).ToArray()).SendAsync(defultData.DemandLimit, signalrResponse);
                            }
                        }

                    }

                }
            }
        }
    }
    public class ResponseSignalRDemandLimitNotification
    {
        public string arabicNote { get; set; }
        public string latinNote { get; set; }
    }
}
