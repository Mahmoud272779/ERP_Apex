using App.Infrastructure.Persistence.Context;
using App.Infrastructure.UserManagementDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.DataBasesMigraiton
{
    public interface iUpdateMigrationForCompanies
    {
        public Task<List<string>> updateDatabases();
    }
    public class updateMigrationForCompanies : iUpdateMigrationForCompanies
    {
        private readonly IConfiguration _configuration;
        ClientSqlDbContext _clientcontext;

        public updateMigrationForCompanies(IConfiguration configuration, ClientSqlDbContext Clientcontext)
        {
            _configuration = configuration;
            _clientcontext = Clientcontext;
        }

        public async Task<List<string>> updateDatabases()
        {
            ERP_UsersManagerContext _userManagementcontext = new ERP_UsersManagerContext(_configuration);
            var databases = _userManagementcontext.UserApplications.Where(x => !string.IsNullOrEmpty(x.DatabaseName)).Select(x => x.DatabaseName).ToList();
            List<string> exceptions = new List<string>();

            foreach (var item in databases)
            {
                var connString =
                                  @"Data Source=" + _configuration["ApplicationSetting:serverName"] + ";" +
                                  "Initial Catalog=" + item + ";" +
                                  "user id=" + _configuration["ApplicationSetting:UID"] + ";" +
                                  "password=" + _configuration["ApplicationSetting:Password"] + ";" +
                                  "MultipleActiveResultSets=true;";


                if (item == "Apex_testsecion_2023_1_20230510222928")
                {
                    var ss = "";

                }


                _clientcontext.Database.SetConnectionString(connString);
                var iisHavePendingMigration = _clientcontext.Database.GetPendingMigrations().Any();
                var _iisHavePendingMigration = _clientcontext.Database.GetPendingMigrations();
                if (_iisHavePendingMigration.Any(x => x == "20230316155926_Publish_16-3-2023"))
                    continue;
                if (iisHavePendingMigration)
                    try
                    {

                        await _clientcontext.Database.MigrateAsync();
                    }
                    catch (Exception ex)
                    {

                        exceptions.Add(ex.Message);
                    }
            }
            return exceptions;
        }
    }
}
