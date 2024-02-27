using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Inv_General_Settings;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.General;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Shared;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class InvGeneralSettingsController : ApiStoreControllerBase
    {
        private readonly IInvGeneralSettingsService GSService;
        private readonly iAuthorizationService _iAuthorizationService;

        public InvGeneralSettingsController(IInvGeneralSettingsService _GSService,iAuthorizationService iAuthorizationService,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            GSService = _GSService;
            _iAuthorizationService = iAuthorizationService;
        }

        
        [HttpPut(nameof(UpdatePurchasesSettings))]
        public async Task<ResponseResult> UpdatePurchasesSettings(PurchasesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings,(int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdatePurchasesSettings(parameter);
            return data;
        }

        
        [HttpPut(nameof(UpdatePOSSettings))]
        public async Task<ResponseResult> UpdatePOSSettings(POSRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdatePOSSettings(parameter);
            return data;
        }

    
        [HttpPut(nameof(UpdateSalesSettings))]
        public async Task<ResponseResult> UpdateSalesSettings(SalesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateSalesSettings(parameter);
            return data;
        }
        
        [HttpPut(nameof(UpdateOtherSettings))]
        public async Task<ResponseResult> UpdateOtherSettings(OtherRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateOtherSettings(parameter);
            return data;
        }
        [HttpPut(nameof(UpdateOtherGeneralSettings))]
        public async Task<ResponseResult> UpdateOtherGeneralSettings(updateOhterGeneralSettingsRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateOtherGeneralSettings(parm);
            return data;
        }
        
        [HttpPut(nameof(UpdateFundsSettings))]
        public async Task<ResponseResult> UpdateFundsSettings(FundsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateFundsSettings(parameter);
            return data;
        }
        
        [HttpPut(nameof(UpdateBarcodeSettings))]
        public async Task<ResponseResult> UpdateBarcodeSettings(BarcodeRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateBarcodeSettings(parameter);
            return data;
        }
        
        [HttpPut(nameof(UpdateVATSettings))]
        public async Task<ResponseResult> UpdateVATSettings(VATRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateVATSettings(parameter);
            return data;
        }
        
        [HttpPut(nameof(UpdateAccrediteSettings))]
        public async Task<ResponseResult> UpdateAccrediteSettings(AccrediteRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateAccrediteSettings(parameter);
            return data;
        }
        
        [HttpPut(nameof(UpdateCustomerDisplaySettings))]
        public async Task<ResponseResult> UpdateCustomerDisplaySettings(CustomerDisplayRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateCustomerDisplaySettings(parameter);
            return data;
        }
        [HttpPut(nameof(updateEmailSettings))]
        public async Task<ResponseResult> updateEmailSettings(emailSettingsDTO parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.updateEmailSettings(parm);
            return data;
        }
        
        [HttpPut(nameof(UpdateElectronicInvoiceSettings))]
        public async Task<ResponseResult> UpdateElectronicInvoiceSettings(ElectronicInvoiceRequest parm)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.GeneralSettings_Settings, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var data = await GSService.UpdateElectronicInvoiceSettings(parm);
            return data;
        } 
        
     


        
        [HttpGet("GetSettings")]
        public async Task<ResponseResult> GetSettings(int insideInvoice=0)
        {
            if (insideInvoice==0)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settings, (int)SubFormsIds.StoreSettings_Settings, Opretion.Open);
                if (isAuthorized != null)
                    return isAuthorized;
            }
           
            var data = await GSService.GetSettings();
           
            return data;
        }
        
        [HttpGet("GetSettingsBySession")]
        public async Task<ResponseResult> GetSettingsBySession()
        {
            var data = await GSService.GetSettingsSession();
            return data;
        }

        
        [HttpGet(nameof(GetPurchasesSettings))]
        public async Task<ResponseResult> GetPurchasesSettings()
        {
            var data = await GSService.GetPurchasesSettings();
            return data;
        }

        
        [HttpGet(nameof(GetPOSSettings))]
        public async Task<ResponseResult> GetPOSSettings()
        {
            var data = await GSService.GetPOSSettings();
            return data;
        }

        
        [HttpGet(nameof(GetSalesSettings))]
        public async Task<ResponseResult> GetSalesSettings()
        {
            var data = await GSService.GetSalesSettings();
            return data;
        }

        
        [HttpGet(nameof(GetOtherSettings))]
        public async Task<ResponseResult> GetOtherSettings()
        {
            var data = await GSService.GetOtherSettings();
            return data;
        }

        
        [HttpGet(nameof(GetBarcodeSettings))]
        public async Task<ResponseResult> GetBarcodeSettings()
        {
            var data = await GSService.GetBarcodeSettings();
            return data;
        }

        
        [HttpGet(nameof(GetCustomerDisplaySettings))]
        public async Task<ResponseResult> GetCustomerDisplaySettings()
        {
            var data = await GSService.GetCustomerDisplaySettings();
            return data;
        }

        
        [HttpGet(nameof(GetFundsSettings))]
        public async Task<ResponseResult> GetFundsSettings()
        {
            var data = await GSService.GetFundsSettings();
            return data;
        }

        
        [HttpGet(nameof(GetVATSettings))]
        public async Task<ResponseResult> GetVATSettings()
        {
            var data = await GSService.GetVATSettings();
            return data;
        }

        
        [HttpGet(nameof(GetAccrediteSettings))]
        public async Task<ResponseResult> GetAccrediteSettings()
        {
            var data = await GSService.GetAccrediteSettings();
            return data;
        }
        [HttpGet(nameof(TestEmailSend))]
        public async Task<ResponseResult> TestEmailSend(string EmailTo)
        {
            var data = await GSService.TestEmailSend(EmailTo);
            return data;
        }
        [HttpGet(nameof(GetEmailSettings))]
        public async Task<ResponseResult> GetEmailSettings()
        {
            var data = await GSService.GetEmailSettings();
            return data;
        }
        [HttpGet(nameof(GetElectronicInvoiceSettings))]
        public async Task<ResponseResult> GetElectronicInvoiceSettings()
        {
            var data = await GSService.GetElectronicInvoiceSettings();
            return data;
        }

      

    }
}
