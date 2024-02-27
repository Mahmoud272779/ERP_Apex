using App.Domain.Enums;
using App.Infrastructure.UserManagementDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Helpers.MiddleWares
{
    public class CompanyPeriodCheckerMiddleware
    {
        private readonly RequestDelegate _next;

        public CompanyPeriodCheckerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var allowedAPI = allowedAPIs.allowedAPIS();
            var endPointName = context.Request.Path.ToString().Split('/').Last().ToLower();
            if (allowedAPI.Where(x => x.APIName.ToLower() == endPointName.ToLower()).Any())
            {
                await _next(context);
                return;
            }
            else
            {
                ERP_UsersManagerContext usersManagerContext = new ERP_UsersManagerContext(configuration);
                var endPoint = context.GetEndpoint();
                var attribute = endPoint?.Metadata.OfType<AllowAnonymousAttribute>();
                if (attribute.Any())
                {
                    await _next(context);
                    return;
                }
                var token = context.GetTokenAsync("access_token").Result;
                if (token == null)
                    errorResponse.responseUnautorized(context);
                var isTechincalSupport = int.Parse(contextHelper.checkIsTechnicalSupport(token));
                if (isTechincalSupport == 1)
                {
                    await _next(context);
                    return;
                }
                var CompanyLoginName = StringEncryption.DecryptString(contextHelper.CompanyLogin(token));

                var companyInfo = usersManagerContext.SubReqPeriods.AsNoTracking()
                                                                   .Include(x => x.Company)
                                                                   .Include(x => x.SubReq)
                                                                   .Where(x => x.Company.CompanyLogin == CompanyLoginName && x.DateTo.Date > DateTime.Now.Date).FirstOrDefault();
                if (companyInfo == null)
                    errorResponse.PeriodEnded(context);
                await _next(context);
            }
        }
    }
    public static class CompanyPeriodChecker
    {
        public static IApplicationBuilder UseCompanyPeriodCheckerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CompanyPeriodCheckerMiddleware>();
        }
    }

}
