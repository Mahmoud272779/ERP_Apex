using App.Infrastructure.Persistence.Context;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.ActivePOSSession
{
    public class ActivePOSSessionHandler : IRequestHandler<ActivePOSSessionRequest, ResponseResult>
    {
        private readonly IConfiguration _configuration;
        private readonly ClientSqlDbContext dbContext;

        public ActivePOSSessionHandler(IConfiguration configuration, ClientSqlDbContext dbContext)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task<ResponseResult> Handle(ActivePOSSessionRequest request, CancellationToken cancellationToken)
        {
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                       $"Initial Catalog={request.databaseName};" +
                                       $"user id={_configuration["ApplicationSetting:UID"]};" +
                                       $"password={_configuration["ApplicationSetting:Password"]};" +
                                       $"MultipleActiveResultSets=true;";
            var con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                con.Execute($"update POSSession set sessionStatus = 1 where employeeId ={request.employeeId} and sessionStatus = 3");
            }
            catch (Exception)
            {
            }
            finally
            {

                con.Close();
            }

            return new ResponseResult() { Code = 200 };
        }
    }
}
