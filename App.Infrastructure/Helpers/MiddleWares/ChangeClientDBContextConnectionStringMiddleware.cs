using App.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public class ChangeClientDBContextConnectionString
    {
        private readonly RequestDelegate _next;

        public ChangeClientDBContextConnectionString(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,ClientSqlDbContext clientSqlDbContext)
        {

            var token = await context.GetTokenAsync("access_token");
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            //var endPoint = context.GetEndpoint();
            //var attribute = endPoint?.Metadata.OfType<AllowAnonymousAttribute>();
            //if (attribute.Any())
            //{
            //    await _next(context);
            //}
            //var token = await context.GetTokenAsync("access_token");
            //if (token == null)
            //    responseUnautorized(context);
            //var databaseName = StringEncryption.DecryptString(contextHelper.GetDBName(token));
            //string connectionString = @"Data Source=41.39.232.30;Initial Catalog=" + databaseName + ";user id=sa;password=newttech123;MultipleActiveResultSets=true;";
            //clientSqlDbContext.Database.SetConnectionString(connectionString);
        }

        private async void responseUnautorized(HttpContext context)
        {
            var obj = new PropUnautorized()
            {
                messageAr = ErrorMessagesAr.NoPermission,
                messageEn = ErrorMessagesEn.NoPermission
            };
            var result = new UnauthorizedObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
        }

        

        
    }
    public class PropUnautorized
    {
        public string messageAr { get; set; }
        public string messageEn { get; set; }
    }
    public static class PermissionMiddleware
    {
        public static IApplicationBuilder ChangeClientDBContextConnectionStringMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ChangeClientDBContextConnectionString>();
        }
    }
}

