using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class AddPurchaseService : BaseClass, IAddPurchaseService
    {
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
         private IAddInvoice generalProcess;
       
        public AddPurchaseService(  IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery, 
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
         
            this.generalProcess = generalProcess;
        }
      
        public async Task<ResponseResult> AddPurchase(InvoiceMasterRequest parameter , int invoiceTypeId)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);
                
           
                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Purchases_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Purchases_PriceIncludeVat,
                    setDecimal=setting.Other_Decimals,
                };
                string InvoiceTypeName = Aliases.InvoicesCode.Purchase;
                string fileDirectory = FilesDirectories.Purchases;
                if (invoiceTypeId==(int)DocumentType.wov_purchase)
                {
                    InvoiceTypeName = Aliases.InvoicesCode.WOV_Purchase;
                    fileDirectory = FilesDirectories.WOV_Purchases;
                }
                var res = await generalProcess.SaveInvoice(parameter, setting, SettingsOfInvoice, invoiceTypeId, InvoiceTypeName, 0, fileDirectory);
                res.isPrint = setting.Purchases_PrintWithSave;
                return  res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }
    }
}
