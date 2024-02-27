using App.Api.Controllers.BaseController;
using App.Application.Services.Process.GeneralSettings;
using App.Domain.Models.Security.Authentication.Request;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class GeneralSettingsController : ApiStoreControllerBase
    {
        private readonly IGeneralSettingsBusiness generalSettingsBusiness;
        //IActionResultResponseHandler
        public GeneralSettingsController(IGeneralSettingsBusiness _generalSettingsBusiness
            , IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            generalSettingsBusiness = _generalSettingsBusiness;
        }

        
        [HttpPost(nameof(AddGeneralSettings))]
        public async Task<IRepositoryResult> AddGeneralSettings([FromBody] GeneralSettingsParameter parameter)
        {
            var data = await generalSettingsBusiness.AddGeneralSettings(parameter);
            var result = ResponseHandler.GetResult(data);
            return result;
        }

        
        [HttpPut(nameof(UpdateGeneralSettings))]
        public async Task<IRepositoryResult> UpdateGeneralSettings([FromBody] UpdateGeneralSettingsParameter parameter)
        {
            var data = await generalSettingsBusiness.UpdateGeneralSettings(parameter);
            var result = ResponseHandler.GetResult(data);
            return result;
        }

        
        [HttpPost(nameof(GetGeneralSettings))]
        public async Task<IRepositoryResult> GetGeneralSettings(int PageNumber, int PageSize)
        {
            PageParameter paramters = new PageParameter()
            {

                PageNumber = PageNumber,
                PageSize = PageSize

            };
            var data = await generalSettingsBusiness.GetGeneralSettings(paramters);
            var result = ResponseHandler.GetResult(data);
            return result;
        }

        


    }
}