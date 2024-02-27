using App.Infrastructure;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class SignalRHelper
    {
        public static string getUserConnectionId(IMemoryCache _memoryCache, IHttpContextAccessor httpContext)
        {
            var usersSignalR = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            if (httpContext == null)
                return null;
            var token = contextHelper.getToken(httpContext);
            if (token == null)
                return null;
            var CompanyLogin = contextHelper.CompanyLogin(token);
            var employeeId = int.Parse(StringEncryption.DecryptString(contextHelper.GetEmployeeId(token)));
            if (CompanyLogin == null || employeeId == 0)
                return null;
            var userConnection = usersSignalR.Where(x => x.CompanyLogin == CompanyLogin && x.EmployeeId == employeeId).FirstOrDefault();
            if (userConnection == null)
                return null;
            return userConnection.connectionId;

        }

    }
}
