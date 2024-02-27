using App.Application.Services.HelperService.EmailServices;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ForgetPassword
{
    public class forgetPasswordHandler : IRequestHandler<forgetPasswordRequest, ResponseResult>
    {
        private readonly IConfiguration _configuration;
        private ERP_UsersManagerContext _userManagementcontext;
        private ClientSqlDbContext _ClientSqlDbContext;
        private readonly IEmailService _emailService;

        public forgetPasswordHandler(IConfiguration configuration, ClientSqlDbContext clientSqlDbContext, IEmailService emailService)
        {
            _configuration = configuration;
            _userManagementcontext = new ERP_UsersManagerContext(configuration);
            _ClientSqlDbContext = clientSqlDbContext;
            _emailService = emailService;
        }
        public async Task<ResponseResult> Handle(forgetPasswordRequest request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.companyLogin) || string.IsNullOrEmpty(request.email))
                return new ResponseResult()
                {
                    Note = "data required",
                    Result = Result.Failed,
                    ErrorMessageAr = "يجب ادخال البيانات المطلبوبه",
                    ErrorMessageEn = "You should enter the required data first"
                };
            var company = _userManagementcontext.UserApplications.Where(x => x.CompanyLogin == request.companyLogin);
            if (!company.Any())
                return new ResponseResult()
                {
                    Note = "Company Is not Exist",
                    Result = Result.Failed,
                    ErrorMessageEn = "This company is not exist",
                    ErrorMessageAr = "هذه الشركة غير موجودة"
                };
            string DBName = company.FirstOrDefault().DatabaseName;
            if(string.IsNullOrEmpty(DBName))
                return new ResponseResult()
                {
                    Note = "Database Is not Exist",
                    Result = Result.Failed,
                    ErrorMessageAr = "خطأ قاعدة البيانات غير موجودة",
                    ErrorMessageEn = "Error Database is not Exist"
                };
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                   $"Initial Catalog={DBName};" +
                                   $"user id={_configuration["ApplicationSetting:UID"]};" +
                                   $"password={_configuration["ApplicationSetting:Password"]};" +
                                   $"MultipleActiveResultSets=true;";
            _ClientSqlDbContext.Database.SetConnectionString(connectionString);
            //check if email is exist 
            var FindUser = _ClientSqlDbContext.userAccount.AsNoTracking().Include(x=> x.employees).Where(x => x.email == request.email);
            if(!FindUser.Any())
                return new ResponseResult()
                {
                    Note = "This Email is not exist",
                    Result = Result.Failed,
                    ErrorMessageEn = "This Email is not exist",
                    ErrorMessageAr = "البريد الالكتروني غير موجود"
                };
            var lastForgetPassword = _ClientSqlDbContext.usersForgetPasswords.Where(x => x.userId == FindUser.FirstOrDefault().id);
            if (lastForgetPassword.Any())
            {
                var timeBetweenForget = DateTime.Now - lastForgetPassword.OrderByDescending(x=> x.date).FirstOrDefault().date;
                var waitingTime = (TimeSpan.FromMinutes(defultData.timeBetweenForget) - timeBetweenForget).TotalMinutes;
                if (timeBetweenForget.TotalMinutes < defultData.timeBetweenForget)
                    return new ResponseResult()
                    {
                        Note = "Wait For one minute to send another email",
                        Result = Result.Failed,
                        ErrorMessageAr = "يجب الانتظار دقيقه واحدة لارسال ايميل اخر",
                        ErrorMessageEn = "You have to wait one minute until the next request",
                        Data = waitingTime
                    };
            }
            var templetPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "EmailTemplet", "emailtemp.html");
            var emailBodyPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "EmailTemplet", "forgetPasswordBody.txt");
            var emailBody = File.ReadAllText(emailBodyPath);
            emailBody = emailBody.Replace("[Username]",FindUser.FirstOrDefault().username).Replace("[Password]",FindUser.FirstOrDefault().password);
            var email = File.ReadAllText(templetPath).Replace("[username]", FindUser.FirstOrDefault().employees.ArabicName).Replace("[Body]", emailBody);
            await _emailService.ForgetPasswordSendEmail(new ForgetPasswordEmail
            {
                Body = email,
                toEmail = request.email
            });
            _ClientSqlDbContext.usersForgetPasswords.Add(new usersForgetPassword()
            {
                date = DateTime.Now,
                userId = FindUser.FirstOrDefault().id
            });
            _ClientSqlDbContext.SaveChanges();
            return new ResponseResult()
            {
                Note = "Email Sent Success",
                Result = Result.Success,
                Data = defultData.timeBetweenForget
            };
        }
    }
}
