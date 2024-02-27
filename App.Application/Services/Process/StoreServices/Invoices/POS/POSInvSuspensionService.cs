using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.POS;
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
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public class POSInvSuspensionService : BaseClass, IPOSInvSuspensionService
    {
        private readonly IRepositoryQuery<POSInvoiceSuspension> POSInvoiceSuspensionRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
        private IAddSuspensionInvoice AddSuspensionInvoiceService;
        private IGetInvSuspensionService GetInvoiceSuspensionService;

        public POSInvSuspensionService(IRepositoryQuery<POSInvoiceSuspension> _POSInvoiceSuspensionRepositoryQuery,
                              IAddSuspensionInvoice _AddSuspensionInvoiceService,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IGetInvSuspensionService _GetInvoiceSuspensionService,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            POSInvoiceSuspensionRepositoryQuery = _POSInvoiceSuspensionRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.AddSuspensionInvoiceService = _AddSuspensionInvoiceService;
            this.GetInvoiceSuspensionService = _GetInvoiceSuspensionService;
        }
        public async Task<ResponseResult> AddSuspensionInvoice(InvoiceSuspensionRequest parameter)
        {
            try
            {

                if (parameter.StoreId == 0)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        ErrorMessageAr = ErrorMessagesAr.SelectStore,
                        ErrorMessageEn = ErrorMessagesEn.SelectStore
                    };
                }
                if (parameter.PersonId == 0)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        ErrorMessageAr = ErrorMessagesAr.SelectCustomer,
                        ErrorMessageEn = ErrorMessagesEn.SelectCustomer
                    };
                }
                if (parameter.InvoiceDetails == null)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        ErrorMessageAr = ErrorMessagesAr.NoItems,
                        ErrorMessageEn = ErrorMessagesEn.NoItems
                    };
                }
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);


                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Pos_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Pos_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };

                var res = await AddSuspensionInvoiceService.SaveSuspensionInvoice(parameter, setting, SettingsOfInvoice, (int)DocumentType.POS, Aliases.InvoicesCode.BindingPOS, 0);

                return res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }

        public async Task<ResponseResult> GetSuspensionInvoice(int? PageNumber, int? PageSize)
        {
            ResponseResult res = await GetInvoiceSuspensionService.GetSuspensionInvoices(PageNumber, PageSize);

            return res;
        }

        public async Task<ResponseResult> GetSuspensionInvoiceById(int Id)
        {
            ResponseResult res = await GetInvoiceSuspensionService.GetSuspensionInvoicesById(Id);

            return res;
        }
    }
}
