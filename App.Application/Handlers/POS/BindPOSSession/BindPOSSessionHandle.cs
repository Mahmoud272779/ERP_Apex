using App.Infrastructure.Persistence.Context;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.BindPOSSession
{
    public class BindPOSSessionHandle : IRequestHandler<BindPOSSessionRequest, ResponseResult>
    {
        private readonly IConfiguration _configuration;
        private readonly ClientSqlDbContext dbContext;

        public BindPOSSessionHandle(IConfiguration configuration, ClientSqlDbContext dbContext)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
        }
        public async Task<ResponseResult> Handle(BindPOSSessionRequest request, CancellationToken cancellationToken)
        {
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                       $"Initial Catalog={request.databaseName};" +
                                       $"user id={_configuration["ApplicationSetting:UID"]};" +
                                       $"password={_configuration["ApplicationSetting:Password"]};" +
                                       $"MultipleActiveResultSets=true;";
            dbContext.Database.SetConnectionString(connectionString);
            var employeeSession = dbContext.pOSSessions.Where(x => x.employeeId == request.employeeId && x.sessionStatus == (int)POSSessionStatus.active).OrderBy(x => x.Id).LastOrDefault();
            if (employeeSession != null)
            {
                employeeSession.sessionStatus = (int)POSSessionStatus.bining;
                dbContext.pOSSessions.Update(employeeSession);
                var saved = dbContext.SaveChanges();
                Console.WriteLine($"start at {DateTime.Now}");
                BackgroundJob.Schedule(() => CloseSessionByHangFire(request.employeeId, request.databaseName), TimeSpan.FromSeconds(5));
            }
            return new ResponseResult()
            {
                Result = Result.Success
            };
        }

        public void CloseSessionByHangFire(int employeeId, string databaseName)
        {
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                       $"Initial Catalog={databaseName};" +
                                       $"user id={_configuration["ApplicationSetting:UID"]};" +
                                       $"password={_configuration["ApplicationSetting:Password"]};" +
                                       $"MultipleActiveResultSets=true;";
            dbContext.Database.SetConnectionString(connectionString);
            var employeeSession = dbContext.pOSSessions.Where(x => x.employeeId == employeeId && x.sessionStatus == (int)POSSessionStatus.bining).OrderBy(x => x.Id).LastOrDefault();
            if (employeeSession == null)
                return;
            employeeSession.sessionStatus = (int)POSSessionStatus.closed;
            employeeSession.end = DateTime.Now;
            employeeSession.sessionClosedById = employeeId;
            dbContext.pOSSessions.Update(employeeSession);
            dbContext.SaveChanges();
            Console.WriteLine($"End at {DateTime.Now}");
        }
    }
}
