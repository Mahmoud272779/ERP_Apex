using App.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Reposiotries.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using App.Infrastructure.Identity;
using App.Infrastructure.Interfaces;
using App.Infrastructure.Persistence.DapperConfiguration;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Interfaces.Context;

using App.Infrastructure.Persistence.Seed;
using App.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Identity.UI.Services;
using App.Infrastructure.EmailService;
using App.Infrastructure.Mapping;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using App.Infrastructure.settings;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authentication;
using App.Infrastructure.UserManagementDB;

namespace App.Infrastructure.DI
{
    public static class DI
    {


        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {


            //var dbName = StringEncryption.DecryptString(contextHelper.GetDBName(context.HttpContext.GetTokenAsync("access_token").Result));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddDbContextPool<ClientSqlDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SQLConnection")));






            services.AddSingleton<IErpInitilizerData, ErpInitilizerData>();
            //master context
            services.AddScoped(typeof(IRepositoryQueryMaster<>), typeof(RepositoryQueryMaster<>));
            //services.AddScoped<Func<ApplicationSqlDbContextMaster>>((provider) => () => provider.GetService<ApplicationSqlDbContextMaster>());
            //services.AddScoped<ApplicationSqlDbContextMaster>();
            services.AddTransient<IApplicationSqlDbContext>(provider => provider.GetService<ClientSqlDbContext>());
            services.AddTransient<IApplicationSqlDbContextMaster>(provider => provider.GetService<ApplicationSqlDbContextMaster>());

            services.AddScoped(typeof(IRepositoryQuery<>), typeof(RepositoryQuery<>));
            services.AddScoped(typeof(IRepositoryCommand<>), typeof(RepositoryCommand<>));
            services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
            services.AddScoped<DbFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMailKite, MailKite>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddScoped<Func<ClientSqlDbContext>>((provider) => () => provider.GetService<ClientSqlDbContext>());
            services.AddScoped<Func<ERP_UsersManagerContext>>((provider) => () => provider.GetService<ERP_UsersManagerContext>());
            services.AddScoped<Func<SqlConnection>>((provider) => () => provider.GetService<SqlConnection>());


            services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();



            services.AddScoped<ClientSqlDbContext>();
            services.AddScoped<ERP_UsersManagerContext>();
            services.AddScoped<SqlConnection>();



            services.AddScoped<IApplicationOracleDbContext>(provider => provider.GetService<ApplicationOracleDbContext>());
            //services.AddTransient<IApplicationSqlDbContext>(provider => provider.GetService<ClientSqlDbContext>());
            services.AddSingleton<IApplicationUser, ApplicationUserData>();
            services.AddHttpClient();
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = defultData.site,
                    ValidateAudience = true,
                    ValidAudience = defultData.site,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(defultData.JWT_SECURITY_KEY)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-NZ");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("en-NZ") };
            });
            services.AddCors();
            return services;
        }
    }
}
