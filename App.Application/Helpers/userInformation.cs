using App.Domain.Entities.POS;
using App.Domain.Entities.Process.General;
using App.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;

namespace App.Application.Helpers
{
    public interface iUserInformation
    {
        public Task<UserInformationModel> GetUserInformation();
        public GetUserByIdModel GetUserInformationById(int id, int? userId = null);
    }
    public class UserInformation : iUserInformation
    {
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryQuery<GLBranch> _gLBranchQuery;
        private readonly IRepositoryQuery<OtherSettingsStores> _otherSettingsStoresQuery;
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly IRepositoryQuery<OtherSettingsSafes> _otherSettingsSafesQuery;
        private readonly IRepositoryQuery<OtherSettingsBanks> _otherSettingsBanksQuery;
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IHttpContextAccessor _httpContext;

        public UserInformation(
                IRepositoryQuery<userAccount> userAccountQuery,
                IRepositoryQuery<GLBranch> GLBranchQuery,
                IRepositoryQuery<OtherSettingsStores> OtherSettingsStoresQuery,
                IRepositoryQuery<OtherSettingsSafes> OtherSettingsSafesQuery,
                IRepositoryQuery<OtherSettingsBanks> OtherSettingsBanksQuery,
                IHttpContextAccessor httpContext,
                IRepositoryQuery<signalR> signalRQuery,
                IRepositoryQuery<POSSession> pOSSessionQuery)
        {
            _userAccountQuery = userAccountQuery;
            _gLBranchQuery = GLBranchQuery;
            _otherSettingsStoresQuery = OtherSettingsStoresQuery;
            _otherSettingsSafesQuery = OtherSettingsSafesQuery;
            _otherSettingsBanksQuery = OtherSettingsBanksQuery;
            _httpContext = httpContext;
            _signalRQuery = signalRQuery;
            _POSSessionQuery = pOSSessionQuery;
        }

        public async Task<UserInformationModel> GetUserInformation()
        {
            var token = await _httpContext.HttpContext.GetTokenAsync("access_token");
            if (token == null)
            {
                token = _httpContext.HttpContext.Request.Query["access_token"];
                if (token == null)
                {
                    errorResponse.responseUnautorized(_httpContext.HttpContext);
                    return null;
                }

            }

            var isTechincalSupport = int.Parse(contextHelper.checkIsTechnicalSupport(token));

            var tokenPayload = contextHelper.DecodingToken(token);


            var userId = int.Parse(StringEncryption.DecryptString(tokenPayload["userID"]));

            //var requestedCurrentBranch = _httpContext.HttpContext.Request.Headers["CurrentBranchId"];

            var userInfo = _userAccountQuery.TableNoTracking
                .Include(x => x.employees)
                .Include(x => x.employees.EmployeeBranches)
                .Include(x => x.otherSettings)
                .Include(x => x.UserAndPermission)

                .Where(x => x.id == userId).FirstOrDefault();
            if (userInfo == null)
            {
                errorResponse.ReponseUserNotFound(_httpContext.HttpContext);
                return null;
            }

            if (userInfo.UserAndPermission.Count() == 0)
            {
                errorResponse.ReponseUserNotFound(_httpContext.HttpContext);
                return null;
            }
            var employeeId = int.Parse(StringEncryption.DecryptString(tokenPayload["employeeId"]));
            var userConnectionId = _signalRQuery.TableNoTracking.Where(x => x.InvEmployeesId == employeeId).FirstOrDefault()?.connectionId ?? string.Empty;
            var currentBranchId = userInfo.employees.EmployeeBranches.Where(x => x.current == true).Select(x => x.BranchId).FirstOrDefault();
            var branches = _gLBranchQuery.TableNoTracking.Where(x => x.Id == currentBranchId);
            var userStores = _otherSettingsStoresQuery.TableNoTracking.Where(x => x.otherSettingsId == userInfo.otherSettings.FirstOrDefault().Id).Select(x => x.InvStpStoresId).ToArray();
            var userSafes = _otherSettingsSafesQuery.TableNoTracking.Where(x => x.otherSettingsId == userInfo.otherSettings.FirstOrDefault().Id).Select(x => x.gLSafeId).ToArray();
            var userBanks = _otherSettingsBanksQuery.TableNoTracking.Where(x => x.otherSettingsId == userInfo.otherSettings.FirstOrDefault().Id).Select(x => x.gLBankId).ToArray();
            var isPOSDesktop = contextHelper.isPOSDesktop(token);
            var posSessionId = _POSSessionQuery.TableNoTracking.Where(x => x.employeeId == employeeId && x.sessionStatus == (int)POSSessionStatus.active).FirstOrDefault()?.Id;

            UserInformationModel resultTaken = new UserInformationModel()
            {
                userId = int.Parse(StringEncryption.DecryptString(tokenPayload["userID"])),
                userPassword = userInfo.password,
                employeeId = employeeId,
                permissionListId = userInfo.UserAndPermission.FirstOrDefault().permissionListId,
                employeeNameAr = userInfo.employees.ArabicName,
                employeeNameEn = userInfo.employees.LatinName,
                employeeImg = userInfo.employees.ImagePath,
                username = userInfo.username,
                browserName = contextHelper.GetBrowserName(_httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString()),
                CurrentbranchId = currentBranchId,
                CurrentbranchCode = branches.FirstOrDefault().Code,
                employeeBranches = userInfo.employees.EmployeeBranches.Select(d => d.BranchId).ToArray(),
                otherSettings = userInfo.otherSettings.FirstOrDefault(),
                userStors = userStores,
                userBanks = userBanks,
                userSafes = userSafes,
                companyLogin = StringEncryption.DecryptString(contextHelper.CompanyLogin(token)),
                signalRConnectionId = userConnectionId,
                token = token,
                isPOSDesktop = isPOSDesktop,
                POSSessionId = posSessionId,
                isTechincalSupport = isTechincalSupport == 1 ? true : false
            };

            return resultTaken;
        }
        public GetUserByIdModel GetUserInformationById(int id, int? userId = null)
        {
            //  var userId = int.Parse(StringEncryption.DecryptString(tokenPayload["userID"]));


            var userInfo = _userAccountQuery.TableNoTracking.Include(x => x.employees).Where(x => x.employeesId == id).FirstOrDefault();
            if (userInfo == null)
                errorResponse.ReponseUserNotFound(_httpContext.HttpContext);
            GetUserByIdModel resultTaken = new GetUserByIdModel()
            {
                employeeNameAr = userInfo.employees.ArabicName,
                employeeNameEn = userInfo.employees.LatinName,
            };
            return resultTaken;
        }


    }
}
public class UserInformationModel
{
    public int userId { get; set; }
    public string userPassword { get; set; }
    public int employeeId { get; set; }
    public int permissionListId { get; set; }
    public string companyLogin { get; set; }
    public object username { get; set; }
    public object employeeNameAr { get; set; }
    public object employeeNameEn { get; set; }
    public string employeeImg { get; set; }
    public object browserName { get; set; }
    public int CurrentbranchId { get; set; }
    public int CurrentbranchCode { get; set; }
    public int[] employeeBranches { get; set; }
    public otherSettings otherSettings { get; set; }
    public int[] userStors { get; set; }
    public int[] userSafes { get; set; }
    public int[] userBanks { get; set; }
    public string signalRConnectionId { get; set; }
    public string token { get; set; }
    public string isPOSDesktop { get; set; }
    public int? POSSessionId { get; set; }
    public bool isTechincalSupport { get; set; }
}
public class GetUserByIdModel
{



    public object employeeNameAr { get; set; }
    public object employeeNameEn { get; set; }
}

