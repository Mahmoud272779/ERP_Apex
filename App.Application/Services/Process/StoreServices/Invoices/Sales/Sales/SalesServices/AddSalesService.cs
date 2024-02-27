using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class AddSalesService : BaseClass, IAddSalesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
         private IAddInvoice generalProcess;
        private IGeneralAPIsService generalAPIsService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvStoreBranch> SotreQuery;


        public AddSalesService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                               IGeneralAPIsService generalAPIsService,
                              IHttpContextAccessor _httpContext,
                              iUserInformation iUserInformation,
                              IRepositoryQuery<InvStoreBranch> sotreQuery) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
            this.generalProcess = generalProcess;
            _iUserInformation = iUserInformation;
            SotreQuery = sotreQuery;
        }

        public async Task<ResponseResult> AddSales(InvoiceMasterRequest parameter)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);


                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Sales_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Sales_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };

                var res = await generalProcess.SaveInvoice(parameter, setting, SettingsOfInvoice, (int)DocumentType.Sales, Aliases.InvoicesCode.Sales,0, FilesDirectories.Sales);
                res.isPrint = setting.Sales_PrintWithSave;
                return  res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }

        public async Task<ResponseResult> AddInvoiceForPOS(InvoiceMasterRequest parameter)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                 var userInfo = await _iUserInformation.GetUserInformation();
                if (userInfo.userStors.Count() ==0)
                {
                    return new ResponseResult()
                    {
                        ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermissionOnStore,
                        ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermissionOnStore,
                        Result = Result.Failed
                    };
                }
                var storeID = SotreQuery.TableNoTracking.Where(h => userInfo.userStors.Contains(h.StoreId) && h.BranchId == userInfo.CurrentbranchId).Select(a=>a.StoreId);//.Select(a => a.StoreBranches);

                // parameter.StoreId = userInfo.userStors.FirstOrDefault();
              if(storeID.Count()==0)
                {
                    return new ResponseResult() { Result = Result.canNotChangeStore, ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermissionOnStore, ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermissionOnStore };
                }
                parameter.StoreId = storeID.First();
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

                var res = await generalProcess.SaveInvoice(parameter, setting, SettingsOfInvoice, (int)DocumentType.POS, Aliases.InvoicesCode.POS, 0, null);

                return res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed ,Note= ex.Message };
            }
        }

        public async Task<ResponseResult> AddOfferPrice(InvoiceMasterRequest parameter)
        {
            try
            {

                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);


                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Sales_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Sales_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };

                var res = await generalProcess.SaveInvoice(parameter, setting, SettingsOfInvoice, (int)DocumentType.OfferPrice, Aliases.InvoicesCode.OfferPrice, 0, "");

                return res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }
    }
}
