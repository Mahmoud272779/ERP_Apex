using App.Domain.Models.Request.General;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Inv_General_Settings
{
    public interface IInvGeneralSettingsService
    {
        Task<ResponseResult> UpdatePurchasesSettings(PurchasesRequest request);
        Task<ResponseResult> UpdatePOSSettings(POSRequest request);
        Task<ResponseResult> UpdateSalesSettings(SalesRequest request);
        Task<ResponseResult> UpdateOtherSettings(OtherRequest request);
        Task<ResponseResult> UpdateOtherGeneralSettings(updateOhterGeneralSettingsRequest parm);
        Task<ResponseResult> updateEmailSettings(emailSettingsDTO parm);
        Task<Other> GetSettingsForprint();
        Task<ResponseResult> UpdateFundsSettings(FundsRequest request);

        Task<ResponseResult> UpdateBarcodeSettings(BarcodeRequest request);

        Task<ResponseResult> UpdateVATSettings(VATRequest request);
        Task<ResponseResult> UpdateAccrediteSettings(AccrediteRequest request);
        Task<ResponseResult> UpdateCustomerDisplaySettings(CustomerDisplayRequest request);
        Task<ResponseResult> UpdateElectronicInvoiceSettings(ElectronicInvoiceRequest request);
        Task<ResponseResult> GetSettings();
      //  Task<ResponseResult> GetVATSetting();
        Task<ResponseResult> GetSettingsSession();
        Task<ResponseResult> GetPurchasesSettings();
        Task<ResponseResult> GetPOSSettings();
        Task<ResponseResult> GetSalesSettings();
        Task<ResponseResult> GetOtherSettings();
        Task<ResponseResult> GetBarcodeSettings();
        Task<ResponseResult> GetCustomerDisplaySettings();
        Task<ResponseResult> GetFundsSettings();
        Task<ResponseResult> GetVATSettings();
        Task<ResponseResult> GetAccrediteSettings();
        Task<ResponseResult> TestEmailSend(string Email);
        Task<ResponseResult> GetEmailSettings();
        Task<ResponseResult> GetElectronicInvoiceSettings();
    }
}
