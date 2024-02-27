using App.Application.Helpers;
using App.Application.Services.HelperService;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class UpdatePurchaseService : BaseClass, IUpdatePurchaseService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IUpdateInvoice GeneralProcessUpdate;
        private readonly IHttpContextAccessor httpContext;
        private SettingsOfInvoice SettingsOfInvoice;
        public UpdatePurchaseService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                           IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IUpdateInvoice _GeneralProcessUpdate,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GeneralProcessUpdate = _GeneralProcessUpdate;
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
        }
        public async Task<ResponseResult> UpdatePurchase(UpdateInvoiceMasterRequest parameter , int invoiceTypeId)
        {
            if (parameter.InvoiceDetails == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.ItemsIsReuired };

            }
            if(parameter.InvoiceId==0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note =Actions.IdIsRequired };


            var setting = await GeneralSettings.SingleOrDefault(a=>a.Id==1);

            
            // validation of total result
          // get purchase which will update
            SettingsOfInvoice = new SettingsOfInvoice();

            SettingsOfInvoice.ActiveDiscount = setting.Purchases_ActiveDiscount;

            string InvoiceTypeName = Aliases.InvoicesCode.Purchase;
            string fileDirectory = FilesDirectories.Purchases;
            if (invoiceTypeId == (int)DocumentType.wov_purchase)
            {
                InvoiceTypeName = Aliases.InvoicesCode.WOV_Purchase;
                fileDirectory = FilesDirectories.WOV_Purchases;
            }

            var res =  await GeneralProcessUpdate.UpdateInvoices(parameter,setting, SettingsOfInvoice, invoiceTypeId, InvoiceTypeName, fileDirectory);
            return res;
        }

    }
}