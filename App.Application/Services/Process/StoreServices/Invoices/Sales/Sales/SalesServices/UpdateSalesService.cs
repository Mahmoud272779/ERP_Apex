using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.SalesServices
{
    public class UpdateSalesService : BaseClass, IUpdateSalesService
    {
         private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IUpdateInvoice GeneralProcessUpdate;
        private readonly IHttpContextAccessor httpContext; 
        private readonly IGetInvoiceByIdService _getInvoiceByIdService; 
        private SettingsOfInvoice SettingsOfInvoice;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvStoreBranch> SotreQuery;

        public UpdateSalesService(IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IUpdateInvoice _GeneralProcessUpdate,
                              IGetInvoiceByIdService getInvoiceByIdService,
                              IHttpContextAccessor _httpContext,
                              iUserInformation iUserInformation,
                              IRepositoryQuery<InvStoreBranch> sotreQuery) : base(_httpContext)
        {
            GeneralProcessUpdate = _GeneralProcessUpdate;
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
            _getInvoiceByIdService = getInvoiceByIdService;
            _iUserInformation = iUserInformation;
            SotreQuery = sotreQuery;
        }
        public async Task<ResponseResult> UpdateSales(UpdateInvoiceMasterRequest parameter)
        {
            
            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);


            // validation of total result
            SettingsOfInvoice = new SettingsOfInvoice();

            SettingsOfInvoice.ActiveDiscount = setting.Sales_ActiveDiscount;
          
            var res = await GeneralProcessUpdate.UpdateInvoices(parameter, setting, SettingsOfInvoice, (int)DocumentType.Sales, Aliases.InvoicesCode.Sales, FilesDirectories.Sales);
            return res;
        }

        public async Task<ResponseResult> UpdateInvoiceForPOS(UpdateInvoiceMasterRequest parameter)
        {

            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired
                 ,ErrorMessageEn=ErrorMessagesEn.InvoiceNotExist,ErrorMessageAr=ErrorMessagesAr.InvoiceNotExist};

            // commented as user should select store
         //   var userInfo = await _iUserInformation.GetUserInformation();
         //   var storeID = SotreQuery.TableNoTracking.Where(h => userInfo.userStors.Contains(h.StoreId) && h.BranchId == userInfo.CurrentbranchId).Select(a => a.StoreId);//.Select(a => a.StoreBranches);

           // parameter.StoreId = storeID.FirstOrDefault();

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);


            // validation of total result
            SettingsOfInvoice = new SettingsOfInvoice();

            SettingsOfInvoice.ActiveDiscount = setting.Pos_ActiveDiscount;
            //SettingsOfInvoice.ActiveVat = parameter.ApplyVat;// setting.Vat_Active;
            //SettingsOfInvoice.PriceIncludeVat = setting.Pos_PriceIncludeVat; // setting.Purchases_PriceIncludeVat;

            var res = await GeneralProcessUpdate.UpdateInvoices(parameter, setting, SettingsOfInvoice, (int)DocumentType.POS, Aliases.InvoicesCode.POS, null);

            if (res.Result == Result.Success)
            {
                InvoiceDto data = await _getInvoiceByIdService.GetInvoiceDto((int)res.Id, false);
                return new ResponseResult { Result = Result.Success, Data = data,Id=data.InvoiceId,Code=data.Code };
            }

            return res;
        }

    }
}
