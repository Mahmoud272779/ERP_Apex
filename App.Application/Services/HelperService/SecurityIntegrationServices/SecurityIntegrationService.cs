using App.Infrastructure;
using App.Infrastructure.UserManagementDB;
using Microsoft.AspNetCore.Authentication;
using static App.Domain.Models.Response.General.AdditionalPrices;

namespace App.Application.Services.HelperService.SecurityIntegrationServices
{
    public class SecurityIntegrationService : ISecurityIntegrationService
    {
        private ERP_UsersManagerContext _userManagementcontext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SecurityIntegrationService(IHttpContextAccessor httpContextAccessor, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _userManagementcontext = new ERP_UsersManagerContext(configuration);
            this.httpContextAccessor = httpContextAccessor;
        }
        public string Token() => httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
        public string companyLogin() => !string.IsNullOrEmpty(Token()) ? contextHelper.CompanyLogin(Token()) : null;
        public async Task<companyInfomation> getCompanyInformation()
        {
            var companyLoginName = StringEncryption.DecryptString(this.companyLogin());
            if (companyLoginName == null)
                return null;
            var isPeriodEnded = Convert.ToBoolean(contextHelper.isPeriodEnded(Token()));

            var companyInfo = _userManagementcontext.UserApplications.Where(x => x.CompanyLogin == companyLoginName).FirstOrDefault();
            var period = _userManagementcontext.SubReqPeriods.Where(x => (!isPeriodEnded ? x.DateFrom < DateTime.Now && x.DateTo > DateTime.Now : true) && x.CompanyId == companyInfo.Id).OrderBy(x=> x.Id).LastOrDefault();
            if (period == null)
                return null;
            var subReq = _userManagementcontext.UserApplicationCashes.Where(x => x.Id == period.SubReqId).FirstOrDefault();
            var subApplications = _userManagementcontext.UserApplicationApps.Where(x => x.ReqId == period.SubReqId).Select(x => x.AppId);
            var apps = _userManagementcontext.Apps.Where(x => subApplications.Contains(x.Id)).Select(x => new Apps
            {
                Id = x.Id,
                appNameAr = x.ArabicName,
                appNameEn = x.LatinName
            }).ToList();
            var bundle = _userManagementcontext.Bundles.Where(x => x.BundleId == subReq.BundlesId).FirstOrDefault();
            var additionalItems = _userManagementcontext.AdditionalPriceSubscriptions.AsNoTracking().Include(c=> c.AdditionalPrice).Where(x => x.SubRequestId == subReq.Id);

            var ExtraUsers = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraUsers).FirstOrDefault()?.Count;
            var extraPOS = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraPOS).FirstOrDefault()?.Count;
            var extraEmployee = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraEmployees).FirstOrDefault()?.Count;
            var extraStores = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraStores).FirstOrDefault()?.Count;
            var extraBranches = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraBranches).FirstOrDefault()?.Count;
            var extraInvoices = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraInvoices).FirstOrDefault()?.Count;
            var extraSuppliers = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraSuppliers).FirstOrDefault()?.Count;
            var extraCustomers = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraCustomers).FirstOrDefault()?.Count;
            var extraItems = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraItems).FirstOrDefault()?.Count;
            var extraOfflinePOS = additionalItems.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraOfflinePOS).FirstOrDefault()?.Count;

            List<AdditionalPrices> additionalPrices = additionalItems.Select(x => new AdditionalPrices
            {
                Id = x.AdditionalPriceId,
                Count = x.Count,
                arabicName = x.AdditionalPrice.ArabicName,
                latinName = x.AdditionalPrice.LatinName

            }).ToList();
            var ResConmapnyInfo = new companyInfomation()
            {
                bundleId = subReq.BundlesId,
                AdditionalPrices = additionalPrices,
                apps = apps,
                companyId = companyInfo.Id,
                companyLogin = companyInfo.CompanyLogin,
                companyName = companyInfo.CompanyNameEn,
                databaseName = companyInfo.DatabaseName,
                Email = companyInfo.Email,
                startPeriod = period.DateFrom,
                endPeriod = period.DateTo,
                periodId = period.Id,
                requestId = period.SubReqId,
                bundleAr = bundle.ArabicName,
                bundleEn = bundle.LatinName,
                Months = subReq.Months,
                periodType = subReq.PeriodType,

                isInfinityNumbersOfUsers = subReq.IsInfinityNumbersOfUsers,
                AllowedNumberOfUser = (ExtraUsers != null ? ExtraUsers.Value : 0) + subReq.AllowedNumberOfUsersOfBundle,

                isInfinityNumbersOfPOS = subReq.IsInfinityNumbersOfPos,
                AllowedNumberOfPOS = (extraPOS != null ? extraPOS.Value : 0) + subReq.AllowedNumberOfPosofBundle,

                isInfinityNumbersOfEmployees = subReq.IsInfinityNumbersOfEmployees,
                AllowedNumberOfEmployee = (extraEmployee != null ? extraEmployee.Value : 0) + subReq.AllowedNumberOfEmployeesOfBundle,

                isInfinityNumbersOfStores = subReq.IsInfinityNumbersOfStores,
                AllowedNumberOfStore = (extraStores != null ? extraStores.Value : 0) + subReq.AllowedNumberOfStoresOfBundle,

                isInfinityNumbersOfApps = subReq.IsInfinityNumbersOfApps,
                AllowedNumberOfApps = subReq.AllowedNumberOfApps,

                isInfinityNumbersOfBranchs = subReq.IsInfinityNumbersOfBranchs,
                AllowedNumberOfBranchs = (extraBranches != null ? extraBranches.Value : 0) + subReq.AllowedNumberOfBranchs,

                isInfinityNumbersOfInvoices = subReq.IsInfinityNumbersOfInvoices,
                AllowedNumberOfInvoices = (extraInvoices != null ? extraInvoices.Value : 0) + subReq.AllowedNumberOfInvoices,

                isInfinityNumbersOfSuppliers = subReq.IsInfinityNumbersOfSuppliers,
                AllowedNumberOfSuppliers = (extraSuppliers != null ? extraSuppliers.Value : 0) + subReq.AllowedNumberOfSuppliers,

                isInfinityNumbersOfCustomers = subReq.IsInfinityNumbersOfCustomers,
                AllowedNumberOfCustomers = (extraCustomers != null ? extraCustomers.Value : 0) + subReq.AllowedNumberOfCustomers,

                isInfinityItems = subReq.IsInfinityItems ?? false,
                AllowedNumberOfItems = (extraItems != null ? (extraItems.Value * 1000) : 0) + subReq.AllowedNumberOfItems ,

                AllowedNumberOfExtraOfflinePOS = extraOfflinePOS??0

            };
            return ResConmapnyInfo;
        }
    }
}
