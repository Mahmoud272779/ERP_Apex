using static App.Domain.Models.Response.General.AdditionalPrices;

namespace App.Application.Services.HelperService.SecurityIntegrationServices
{
    public interface ISecurityIntegrationService
    {
        public Task<companyInfomation> getCompanyInformation();
    }
}
