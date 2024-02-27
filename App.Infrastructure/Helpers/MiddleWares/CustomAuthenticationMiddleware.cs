using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace App.Infrastructure
{
    public class CustomAuthentication
    {
        private readonly RequestDelegate _next;

        public CustomAuthentication(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context,IConfiguration configuration)
        {

            var usermanagerContext = new ERP_UsersManagerContext(configuration);

                if (context == null)
                    await _next(context);
                var allowedAPI = allowedAPIs.allowedAPIS();
                var endPointName = context.Request.Path.ToString().Split('/').Last().ToLower();

                if (allowedAPI.Where(x => x.APIName.ToLower() == endPointName.ToLower()).Any())
                {
                    await _next(context);
                    return;
                }
                var token = await context.GetTokenAsync("access_token");
                if (token != null)
                {
                    var isTechincalSupport = int.Parse(contextHelper.checkIsTechnicalSupport(token));
                    if (isTechincalSupport == 1)
                    {
                        await _next(context);
                        return;
                    }
                }
            var DBName = StringEncryption.DecryptString(contextHelper.GetDBName(token));

            SqlConnection con = new SqlConnection(ConnectionString.connectionString(configuration, DBName));
            try
            {
                
                usermanagerContext.ERplogs.Add(new ERplog
                {
                    ApiPath = context.Request.Path.ToString(),
                    Date = DateTime.Now,
                    Token = token
                });
                await usermanagerContext.SaveChangesAsync();
                var endPoint = context.GetEndpoint();
                var attribute = endPoint?.Metadata.OfType<AllowAnonymousAttribute>();
                if (attribute == null)
                {
                    errorResponse.responseUnautorized(context);
                    return;
                }
                if (defultData.isAppAuth)
                {
                    if (attribute.Any())
                    {
                        await _next(context);
                        return;
                    }

                    if (token != null)
                    {
                        var userId = StringEncryption.DecryptString(contextHelper.GetUserID(token));
                  

                        con.Open();
                        IEnumerable<signinLogs> usrTokens = await con.QueryAsync<signinLogs>(MiddlewaresQueries.usrTokens(userId));
                        var usr = usrTokens.Where(c => c.token == token).FirstOrDefault();
                        if (usrTokens.Where(c => c.token == token).FirstOrDefault().stayLoggedin)
                        {
                            await _next(context);
                            return;
                        }
                        if (usrTokens.OrderBy(x => x.Id).Where(c => !c.stayLoggedin).LastOrDefault().token != token)
                        {
                            errorResponse.UserLoggedInFromAnotherPLace(context);
                            return;
                        }
                        IEnumerable<userAccount> isTokenValid = await con.QueryAsync<userAccount>(MiddlewaresQueries.isTokenValid(token));
                        if (!isTokenValid.Any())
                        {
                            errorResponse.responseUnautorized(context);
                        }
                        await _next(context);
                        return;
                    }
                    else
                    {
                        errorResponse.responseUnautorized(context);
                        return;
                    }
                }
                else
                {
                    await _next(context);

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
    public static class customAuthentication
    {
        public static IApplicationBuilder CustomAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthentication>();
        }
    }
}
