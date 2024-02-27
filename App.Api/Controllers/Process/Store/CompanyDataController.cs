using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Company_data;
using App.Domain.Entities;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class CompanyDataController: ApiStoreControllerBase
    {
        readonly private ICompanyDataService CompanyDataService;
        private readonly iAuthorizationService _iAuthorizationService;

        public CompanyDataController(ICompanyDataService _CompanyDataService
                        ,iAuthorizationService iAuthorizationService
                         ,IActionResultResponseHandler ResponseHandler):base(ResponseHandler)
        {
            CompanyDataService = _CompanyDataService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpGet("GetCompanyData")]
        public async Task<ResponseResult> GetCompanyData()
        {
        //    var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.CompanyInformation_Settings, Domain.Enums.Opretion.Open);
        //    if (isAuthorized != null)
        //        return isAuthorized;

            var result = await CompanyDataService.GetCompanyData();
            return result;
        }

        
        [HttpPut(nameof(UpdateCompanyData))]
        public async Task<ResponseResult> UpdateCompanyData([FromForm]UpdateCompanyDataRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.CompanyInformation_Settings, Domain.Enums.Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await CompanyDataService.UpdateCompanyData(parameter);
            return result;
        }
    }
}
