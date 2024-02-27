using App.Domain.Enums;
using App.Domain.Models.Shared;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using App.Infrastructure.Helpers;
using Dapper;
using App.Domain.Entities.Process;
using App.Domain.Entities;

namespace App.Infrastructure
{
    public class AutoLogoutMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoLogoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IConfiguration configuration)
        {
            var token = await context.GetTokenAsync("access_token");
            if (token == null)
            {
                await _next(context);
            }
            else
            {
                var endPointName = context.Request.Path.ToString().Split('/').Last().ToLower();
                if (endPointName == "NotificationHub")
                {
                    await _next(context);
                    return;
                }
                var isTechincalSupport = int.Parse(contextHelper.checkIsTechnicalSupport(token));
                if (isTechincalSupport == 1)
                {
                    await _next(context);
                    return;
                }
                var dbName = StringEncryption.DecryptString(contextHelper.GetDBName(token));
                SqlConnection con = new SqlConnection(ConnectionString.connectionString(configuration, dbName));


                try
                {
                    con.Open();
                    IEnumerable<InvGeneralSettings> invGeneralSettings = await con.QueryAsync<InvGeneralSettings>(MiddlewaresQueries.InvGeneralSettings_autoLogoutInMints());
                    if (invGeneralSettings.FirstOrDefault().autoLogoutInMints == 0)
                    {
                        await _next(context);
                    }
                    else
                    {
                        IEnumerable<signinLogs> userToken = await con.QueryAsync<signinLogs>(MiddlewaresQueries.usrTokens_FilterWithToken(token));
                        if (userToken.First().stayLoggedin)
                        {
                            await _next(context);
                            return;
                        }
                        if (endPointName == "resumesession")
                            await _next(context);
                        else if (userToken.FirstOrDefault().isLocked)
                        {
                            errorResponse.UserIsLocked(context);
                        }
                        else
                        {
                            var timeBetweenCurrentActionAndLastAction = (DateTime.Now - userToken.FirstOrDefault().lastActionTime).TotalMinutes;
                            if (invGeneralSettings.FirstOrDefault().autoLogoutInMints < timeBetweenCurrentActionAndLastAction)
                            {
                                await con.ExecuteAsync(MiddlewaresQueries.updateIsLocked_signinLogs(1, userToken.FirstOrDefault().Id));
                                errorResponse.UserIsLocked(context);
                            }
                            else
                            {
                                await con.ExecuteAsync(MiddlewaresQueries.updateLastActionTime_signinLogs(userToken.FirstOrDefault().Id));
                                await _next(context);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    con.Close();

                }

            }


        }
    }
    public static class AutoLogout
    {
        public static IApplicationBuilder UseAutoLogoutMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoLogoutMiddleware>();
        }
    }
}
