using Hangfire.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace App.Api
{
    public class Startup
    {
        //comment by ahmed atif 18-1
        public static int ProgressCount = 0;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
         



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            
            AppDI.AddApplicationDI(services);
            DI.AddInfrastructureDI(services, Configuration);

            //StringEncryption.DecryptString(contextHelper.GetDBName(HttpContext.GetTokenAsync("access_token").Result)));
            //var provider = services.BuildServiceProvider();

            //var scopeFactory = (IServiceScopeFactory)provider.GetService(typeof(IServiceScopeFactory));

            //services.AddHangfire(x =>
            //{
            //    x.UseSqlServerStorage(Configuration.GetConnectionString("SQLConnection"));
            //    x.UseActivator<AspNetCoreJobActivator>(new AspNetCoreJobActivator(scopeFactory));
            //});
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("SQLConnection")));
            services.AddHangfireServer();
            services.Configure<ApplicationSetup>(Configuration.GetSection("ApplicationSetting"));
            services.AddMvc().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddMvc().AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });//hamada
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddHttpClient("MyCilent", client =>
            {
                client.Timeout = TimeSpan.FromHours(30);
            });
            services.AddSignalR();
            services.AddSingleton<TimerManager>();

            //options => {
            //options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
            // }
            //services.AddSpaStaticFiles(configuration =>
            //    {
            //        configuration.RootPath = "ClientApp/dist";
            //    });
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMemoryCache();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Apex ERP Project", Version = "v1.2.0" });
                options.OperationFilter<SwaggerFilter>();
                options.CustomSchemaIds(type => type.ToString().Replace("+", "."));// A solution for The same schemaId is already used for type(bla bla bla )exception
                //options.DescribeAllEnumsAsStrings();
                var filePath = Path.Combine("C:\\", "App.Api.xml");
                if(!File.Exists(filePath))
                    File.Create(filePath).Close();
                options.IncludeXmlComments(filePath);

                // Swagger 2.+ support
                var security = new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                    //{"Bearer", new string[] { }},
                };


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(security);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ClientSqlDbContext context, IHostApplicationLifetime lifetime, IDistributedCache cache)
        {


    

            var allowSwagger = Configuration.GetSection("ApplicationSetting:Swagger").Value == "True" ? true : false;
            if (allowSwagger)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex ERP API V1/" + context.Connection.Database.ToString());
                    c.DocumentTitle = "Apex ERP Documentation";
                    c.DocExpansion(DocExpansion.None);
                });
            }


            //memory cash
            //lifetime.ApplicationStarted.Register(() =>
            //{
            //    var currentTimeUTC = DateTime.UtcNow.ToString();
            //    byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
            //    var options = new DistributedCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(1));
            //    cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
            //});
           

            app.UseCors(x => x
                             .AllowAnyMethod()
                             .WithOrigins()
                             .AllowAnyHeader()
                             .SetIsOriginAllowed(origin => true)
                             .AllowCredentials()); // allow any origin
                                                   //.AllowCredentials()// allow credentials
                                                   //.AllowAnyOrigin());
            //app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            #region Custom Middle Ware
            app.CustomAuthenticationMiddleware();
            app.UseCompanyPeriodCheckerMiddleware();
            app.UseAutoLogoutMiddleware();
            #endregion
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseFastReport();
            app.UseHangfireDashboard("/HangFireDashboard");

            //signalr
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<HubConfig>("/progressBar");
                endpoints.MapHub<NotificationHub>("/NotificationHub");
                endpoints.MapRazorPages();
            });
        }
    }
}
