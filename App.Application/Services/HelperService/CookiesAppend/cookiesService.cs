using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.CookiesAppend
{
    public class cookiesService : ICookiesService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public cookiesService(Microsoft.Extensions.Configuration.IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> isUpToDate()
        {
            var currentProjectUpdateNumber = _configuration["projectVersion:updateNumber"];
            var clientProjectUpdateNumber = _httpContextAccessor.HttpContext.Request.Cookies["updateNumber"];
            if (clientProjectUpdateNumber == null)
                return false;
            if(int.Parse(currentProjectUpdateNumber) > int.Parse(clientProjectUpdateNumber))
                return false;
            return true;
        }

        public async Task<Tuple<string,string>> ProjectVersion()
        {
            string currentProjectVersion = _configuration["projectVersion:version"];
            string currentProjectUpdate = _configuration["projectVersion:updateNumber"];
            return new Tuple<string,string>(currentProjectVersion, currentProjectUpdate);
        }
    }
}
