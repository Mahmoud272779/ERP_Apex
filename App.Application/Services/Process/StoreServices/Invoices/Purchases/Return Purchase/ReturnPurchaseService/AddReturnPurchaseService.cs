using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Return_Purchase.ReturnPurchaseService
{
   public class AddReturnPurchaseService : BaseClass, IAddReturnPurchaseService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;

        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;

        private IAddInvoice AddInvoiceService;
        private IGeneralAPIsService GeneralAPIsService;
        public AddReturnPurchaseService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
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
        public async Task<ResponseResult> AddReturnPurchase(InvoiceMasterRequest parameter,int invoiceTypeId)
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
                if (!string.IsNullOrEmpty( parameter.ParentInvoiceCode ))  // return with invoice
                {
                    // use old settings of the invoice
                    var MainInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceType == parameter.ParentInvoiceCode.Trim() || a.Code.ToString() == parameter.ParentInvoiceCode.Trim()).ToList();
                    if (MainInvoice.Count() == 0)
                        return new ResponseResult() { Result = Result.NotExist, ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist, ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
                    parameter.isExpenses = MainInvoice.First().isExpenses;
                    SettingsOfInvoice = new SettingsOfInvoice
                    {
                        ActiveDiscount =setting.Purchases_ActiveDiscount,
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
                        setDecimal=setting.Other_Decimals

                    };

                }
                string InvoiceTypeName = Aliases.InvoicesCode.ReturnPurchase;
                string fileDirectory = FilesDirectories.ReturnPurchases;
                if (invoiceTypeId == (int)DocumentType.ReturnWov_purchase)
                {
                    InvoiceTypeName = Aliases.InvoicesCode.ReturnWOV_Purchase;
                    fileDirectory = FilesDirectories.ReturnWOV_Purchases;
                }

              
                var res = await AddInvoiceService.SaveInvoice(parameter, setting, SettingsOfInvoice,
                               invoiceTypeId, InvoiceTypeName, MainInvoiceId, fileDirectory);
                //HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, lastTransactionAction: "U", null, invoice.BookIndex, invoice.Code


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
