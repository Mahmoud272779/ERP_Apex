using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace App.Infrastructure.Helpers
{
    public static class ConnectionString
    {
        public static string connectionString(IConfiguration configuration,string DBName) => $"Data Source={configuration["ApplicationSetting:serverName"]};" +
                                                   $"Initial Catalog={DBName};" +
                                                   $"user id={configuration["ApplicationSetting:UID"]};" +
                                                   $"password={configuration["ApplicationSetting:Password"]};" +
                                                   $"MultipleActiveResultSets=true;";
    }
}
