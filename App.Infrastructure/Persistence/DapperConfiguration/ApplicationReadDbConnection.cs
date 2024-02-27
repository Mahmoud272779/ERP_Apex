using App.Infrastructure.Interfaces;
using App.Infrastructure.Interfaces.Context;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.DapperConfiguration
{
    public class ApplicationReadDbConnection : IApplicationReadDbConnection, IDisposable
    {
        private readonly IDbConnection connection;

        public ApplicationReadDbConnection(IApplicationSqlDbContext Context)
        {
            connection = Context.Connection;
        }
        public void Dispose()
        {
            connection.Dispose();
        }
       
        ~ApplicationReadDbConnection()
        {
            //Dispose();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return (await connection.QueryAsync<T>(sql, param, transaction)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }
        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return await connection.QuerySingleAsync<T>(sql, param, transaction);
        }
    }
}
