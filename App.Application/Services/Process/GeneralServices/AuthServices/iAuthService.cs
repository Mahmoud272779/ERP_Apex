using App.Domain.Models.Security.Authentication.Response;
using System.Threading.Tasks;

namespace App.Application
{
    public interface iAuthService
    {
        public Task<JwtAuthResponse> getAuthToken(string employeeId, userInfo userInfo, string RoleID,string databaseName, string EndPeriodOnEndPeriodOn,string companyLogin, bool isPOSDesktop,bool isPeriodEnded,bool isTechnicalSupport);
    }
}
