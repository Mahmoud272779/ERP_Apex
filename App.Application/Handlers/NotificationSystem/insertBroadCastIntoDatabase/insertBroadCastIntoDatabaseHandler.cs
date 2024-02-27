using Dapper;
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

namespace App.Application.Handlers.NotificationSystem.insertBroadCastIntoDatabase
{
    public class insertBroadCastIntoDatabaseHandler : IRequestHandler<insertBroadCastIntoDatabaseRequiest, bool>
    {
        private readonly IConfiguration _configuration;

        public insertBroadCastIntoDatabaseHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(insertBroadCastIntoDatabaseRequiest request, CancellationToken cancellationToken)
        {
            SqlConnection con = new SqlConnection(ConnectionString.connectionString(_configuration, request.dbName));
            con.Open();
            var query = "insert into NotificationsMaster ";
            query += "(";
            query += "title,";
            query += "Desc,";
            query += "isSystem,";
            if (request.emplployeeId > 0)
                query += "specialUserId";
            query += ") values (" +
                $"{request.title}," +
                $"{request.desc}," +
                $"{request.isSystem},";
            if (request.emplployeeId > 0)
                query += $"{request.emplployeeId},";
            query += ")";

            var insert = await con.ExecuteAsync(query);
            con.Close();
            return insert > 0 ? true : false;
        }
    }
}
