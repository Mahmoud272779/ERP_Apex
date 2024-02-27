using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
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

namespace App.Application
{
    public  class AddReturnSalesService :BaseClass, IAddReturnSalesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;

        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;

        private IAddInvoice AddInvoiceService;
        private IGeneralAPIsService GeneralAPIsService;
        public AddReturnSalesService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice AddInvoiceService,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery,
                              IGeneralAPIsService GeneralAPIsService,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.AddInvoiceService = AddInvoiceService;
            this.InvoiceDetailsRepositoryQuery = InvoiceDetailsRepositoryQuery;
            this.GeneralAPIsService = GeneralAPIsService;
        }

        public async Task<ResponseResult> AddReturnSales(InvoiceMasterRequest parameter)
        {
            try
            {

                if (parameter.InvoiceDetails == null)
                {
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "you should send at least one item" };

                }
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

                //setting validation


                var MainInvoiceId = 0;
                if (!string.IsNullOrEmpty(parameter.ParentInvoiceCode))  // return with invoice
                {

                    // use old settings of the invoice
                    var MainInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceType == parameter.ParentInvoiceCode.Trim() || a.Code.ToString() == parameter.ParentInvoiceCode.Trim()).ToList();
                    if (MainInvoice.Count() ==0)
                        return new ResponseResult() { Result = Result.NotExist, ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist, ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
                 
                    SettingsOfInvoice = new SettingsOfInvoice
                    {
                        ActiveDiscount = setting.Sales_ActiveDiscount,
                        ActiveVat = MainInvoice.First().ApplyVat,
                        PriceIncludeVat = MainInvoice.First().PriceWithVat,
                        setDecimal = MainInvoice.First().RoundNumber,
                    };
                  

                    MainInvoiceId = MainInvoice.First().InvoiceId;
                 

                }
                else  // return without invoice
                {
                    SettingsOfInvoice = new SettingsOfInvoice
                    {
                        ActiveDiscount = setting.Purchases_ActiveDiscount,
                        ActiveVat = setting.Vat_Active,
                        PriceIncludeVat = setting.Purchases_PriceIncludeVat,
                        setDecimal=setting.Other_Decimals,
                    };

                }

                // save return sales 
                var res = await AddInvoiceService.SaveInvoice(parameter, setting, SettingsOfInvoice, (int)DocumentType.ReturnSales, Aliases.InvoicesCode.ReturnSales, MainInvoiceId, FilesDirectories.ReturnSales);
                return res;
                //  return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
            }

            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }

    }
}
