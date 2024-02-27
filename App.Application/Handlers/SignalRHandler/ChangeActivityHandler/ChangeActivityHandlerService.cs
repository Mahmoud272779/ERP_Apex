using App.Domain.Entities.Process.General;
using App.Infrastructure;
using App.Infrastructure.Persistence.Context;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.SignalRHandler.ChangeActivityHandler
{
    public class ChangeActivityHandlerService : IRequestHandler<ChangeActivityHandlerRequest, ResponseResult>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public ChangeActivityHandlerService(IConfiguration configuration, IMemoryCache memoryCache)
        {

            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseResult> Handle(ChangeActivityHandlerRequest request, CancellationToken cancellationToken)
        {
            var _cashHelper = new MemoryCashHelper(_memoryCache);
            var userSignalRInfo = _cashHelper.GetSignalRCashedValues().Where(x => x.connectionId == request.connectionId).FirstOrDefault();
            if (userSignalRInfo == null)
                return new ResponseResult();
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                       $"Initial Catalog={userSignalRInfo.DBName};" +
                                       $"user id={_configuration["ApplicationSetting:UID"]};" +
                                       $"password={_configuration["ApplicationSetting:Password"]};" +
                                       $"MultipleActiveResultSets=true;";
            var con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                string query = $"insert into [signalR] (connectionId,InvEmployeesId,isOnline) (select '{request.connectionId}',{userSignalRInfo.EmployeeId},1 where not exists(select Id from [signalR] where InvEmployeesId ={userSignalRInfo.EmployeeId}));";
                query += $"update signalR set isOnline = {(request.isActive ? 1 : 0)},connectionId = '{request.connectionId}' where InvEmployeesId = {userSignalRInfo.EmployeeId} ;";
                con.Execute(query);
            }
            catch (Exception)
            {

            }
            finally
            {
                con.Close();
            }
            return new ResponseResult();
        }
    }
}
