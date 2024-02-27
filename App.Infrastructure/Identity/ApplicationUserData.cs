using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.Identity
{
    public class ApplicationUserData : IApplicationUser
    {
        //private readonly IHttpContextAccessor _Accessor;
        private IHttpContextAccessor _Accessor;


        public ApplicationUserData(IHttpContextAccessor _httpContextAccessor)
        {
            if (_httpContextAccessor != null)
                _Accessor = _httpContextAccessor;

        }
        public GetUserDataResponse _ApplicationUserData;

        public GetUserDataResponse _ApplicationUser
        {
            get
            {
                //_ApplicationUserData = _Accessor.HttpContext.Session.GetObject<GetUserDataResponse>("UserData"); ;
                //if (_ApplicationUserData == null)
                //    throw new Exception("Login");
                return _ApplicationUserData;

            }
            set
            {
                _ApplicationUserData = _Accessor.HttpContext.Session.GetObject<GetUserDataResponse>("UserData");

            }
        }
        public void SetHttpContextAccessor(IHttpContextAccessor accessor)
        {
            _Accessor = accessor;
        }
    }
}
