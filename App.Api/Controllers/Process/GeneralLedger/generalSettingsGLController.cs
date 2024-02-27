using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.GeneralSettings;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class generalSettingsGLController : ApiGeneralLedgerControllerBase
    {
        private readonly GL_IGeneralSettingsBusiness generalSettingsBusiness;
        private readonly iAuthorizationService _iAuthorizationService;

        public generalSettingsGLController(GL_IGeneralSettingsBusiness _generalSettingsBusiness,iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            generalSettingsBusiness = _generalSettingsBusiness;
            _iAuthorizationService = iAuthorizationService;
        }


        [HttpPut(nameof(updateGeneralSettingsGL))]
        public async Task<IActionResult> updateGeneralSettingsGL([FromBody] UpdateGeneralSettingsParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings,(int)SubFormsIds.GeneralLedgersSettings_GL,Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var data = await generalSettingsBusiness.UpdateGeneralSettings(parameter);
            var result = ResponseHandler.GetResult(data);
            if (result.Success)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
        }

        [HttpGet(nameof(getGeneralSettingsGL))]
        public async Task<IActionResult> getGeneralSettingsGL()
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Open);
            //if (isAuthorized != null)
            //    return Ok(isAuthorized);
            var data = await generalSettingsBusiness.GetGLGeneralSettings();
            var result = ResponseHandler.GetResult(data);
            if (result.Success)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
        }


        [HttpGet(nameof(getPurchasesSettings))]
        public async Task<ResponseResult> getPurchasesSettings()//type is 1 purchase 2 sales
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.GetPurchaseAndSalesData(1);
        }
        [HttpGet(nameof(getSalesSettings))]
        public async Task<ResponseResult> getSalesSettings()//type is 1 purchase 2 sales
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.GetPurchaseAndSalesData(2);
        }
        [HttpGet(nameof(getSettlementsSettings))]
        public async Task<ResponseResult> getSettlementsSettings()//type is 1 purchase 2 sales
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.GetPurchaseAndSalesData(3);
        }
        [HttpGet(nameof(getFundsSettings))]
        public async Task<ResponseResult> getFundsSettings()//type 4 funds
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.GetPurchaseAndSalesData(4);
        }


        [HttpPut(nameof(updatePurchasesSettings))]
        public async Task<ResponseResult> updatePurchasesSettings([FromBody]GLsettingInvoicesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.UpdatePurchaseGeneralSettings(parameter,1);
        }
        [HttpPut(nameof(updateSalesSettings))]
        public async Task<ResponseResult> updateSalesSettings([FromBody]GLsettingInvoicesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.UpdatePurchaseGeneralSettings(parameter,2);
        }
        [HttpPut(nameof(updateSettlementsSettings))]
        public async Task<ResponseResult> updateSettlementsSettings([FromBody]GLsettingInvoicesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.UpdatePurchaseGeneralSettings(parameter,3);
        }
        [HttpPut(nameof(updateFundsSettings))]
        public async Task<ResponseResult> updateFundsSettings([FromBody]GLsettingInvoicesParameter parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.UpdatePurchaseGeneralSettings(parameter,4);
        }
        [HttpPut(nameof(updateCustomerSettings))]
        public async Task<ResponseResult> updateCustomerSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter,SubFormsIds.Customers_Sales);
        }
        [HttpPut(nameof(updateSuppliersSettings))]
        public async Task<ResponseResult> updateSuppliersSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.Suppliers_Purchases);
        }
        [HttpPut(nameof(updateSafesSettings))]
        public async Task<ResponseResult> updateSafesSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.Safes_MainData);
        }
        [HttpPut(nameof(updateBanksSettings))]
        public async Task<ResponseResult> updateBanksSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.Banks_MainData);
        }
        [HttpPut(nameof(updateSalesmanSettings))]
        public async Task<ResponseResult> updateSalesmanSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.Salesmen_Sales);
        }

        [HttpPut(nameof(updateOtherAuthoritiesSettings))]
        public async Task<ResponseResult> updateOtherAuthoritiesSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.OtherAuthorities_MainData);
        }


        [HttpPut(nameof(updateEmployeesSettings))]
        public async Task<ResponseResult> updateEmployeesSettings([FromBody] updateFinancialAccountRelationSettings parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralLedgersSettings_GL, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            return await generalSettingsBusiness.MainDataIntegration(parameter, SubFormsIds.Employees_MainUnits);
        }


        [HttpGet(nameof(getFinancialAccountRelationSettings))]
        public async Task<ResponseResult> getFinancialAccountRelationSettings([FromQuery] getFinancialAccountRelationRequest parameter)
        {
            return await generalSettingsBusiness.getFinancialAccountRelationSettings(parameter);
        }
    }
}
