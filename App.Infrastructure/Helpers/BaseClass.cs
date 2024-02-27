using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure.Identity;
using App.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace App.Infrastructure.Helpers
{
    public class BaseClass : ApplicationUserData
    {
        public IApplicationUser userData { get; set; }
        private static IHttpContextAccessor _httpContextAccessor;

        public BaseClass(IHttpContextAccessor httpContextAccessor) : base(_httpContextAccessor)
        {
            if (httpContextAccessor != null)
                _httpContextAccessor = httpContextAccessor;

        }
        public BaseClass(IApplicationUser _userData, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            userData = _userData;
            _httpContextAccessor = httpContextAccessor;
        }
        public GetUserDataResponse ApplicationUser
        {
            get
            {
                ApplicationUserData userData = new ApplicationUserData(_httpContextAccessor);
                return userData._ApplicationUser;
            }
        }
        public bool IsDevelopmentMode
        {
            get
            {
                string EnvorEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (EnvorEnvironment.ToLower() == "development")
                    return true;
                else
                    return false;
            }
        }







    }
}
