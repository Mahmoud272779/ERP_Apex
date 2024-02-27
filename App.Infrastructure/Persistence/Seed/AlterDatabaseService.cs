using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace App.Application.Helpers.AlterDatabaseServices
{
    public class AlterDatabaseService
    {
        public static string GenerateConnectionString(IConfiguration configuration,string databaseName)
        {
            if(databaseName == null ) throw new ArgumentNullException(nameof(databaseName));
            if(configuration == null ) throw new ArgumentNullException(nameof(configuration));
            var connectionString = $"Data Source={configuration["ApplicationSetting:serverName"]};" +
                                   $"Initial Catalog={databaseName};" +
                                   $"user id={configuration["ApplicationSetting:UID"]};" +
                                   $"password={configuration["ApplicationSetting:Password"]};" +
                                   $"MultipleActiveResultSets=true;";
            return connectionString;
        }
        public static void setColumnAutoIncremintingOnOff(string tableName,string dbName, IConfiguration configuration,bool on)
        { 
            var conn = GenerateConnectionString(configuration, dbName);
            SqlConnection con = new SqlConnection(conn);
            
            
            try
            {
                con.Open();
                var query = $"SET IDENTITY_INSERT {tableName} {(on ? "ON" : "OFF")};";
                var ss = con.Execute(query);
            }
            catch (Exception ex)
            {
                var exx = ex.Message;
                

            }
            finally
            {
                con.Close();
            }
        }
        public static void skipMigration(string dbName, IConfiguration configuration)
        {
            SqlConnection con = new SqlConnection(GenerateConnectionString(configuration, dbName));
            try
            {
                con.Open();
                var migrationData = con.Query<MigrationTable>("select * FROM __EFMigrationsHistory");
                if (migrationData != null)
                {
                    if (migrationData.Any(c => c.MigrationId == "20230719084757_InitialDatabase") && migrationData.Any(c => c.MigrationId == "20231003102709_update_version_2"))
                    {
                        con.Execute("delete from __EFMigrationsHistory");

                        con.Execute($"insert into __EFMigrationsHistory (MigrationId,ProductVersion) values ('20231008092717_InitialDatabase','6.0.5')");

                    }
                }
            }
            catch(Exception ex)
            {
              
            }
            finally
            {
                con.Close();
            }
            
            
        }
    }
  
    public class MigrationTable
    {
        public string MigrationId { get; set; }
        public string ProductVersion { get; set; }
    }
}
