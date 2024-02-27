using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.StoreServices.Invoices.Funds.ItemsFund.ItemFundGLIntegrationServices;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
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

namespace App.Application.Services.Process.StoreServices.Invoices.ItemsFund.ItemsFundsServices
{
    public class AddItemsFundService : BaseClass , IAddItemsFundService
    {

        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly iItemFundGLIntegrationService _iItemFundGLIntegrationService;
        private IAddInvoice generalProcess;

        public AddItemsFundService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              IGeneralAPIsService _GeneralAPIsService,
                              iItemFundGLIntegrationService iItemFundGLIntegrationService,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            _systemHistoryLogsService = systemHistoryLogsService;
            generalAPIsService = _GeneralAPIsService;
            _iItemFundGLIntegrationService = iItemFundGLIntegrationService;
            this.generalProcess = generalProcess;
        }

        public async Task<ResponseResult> AddItemsFund(InvoiceMasterRequest parameter)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                if (parameter.InvoiceDetails == null)
                {
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "you should send at least one item" };

                }
                parameter.InvoiceDate = generalAPIsService.serverDate(parameter.InvoiceDate);
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

                //int NextCode = 1;
                //var codeExist = InvoiceMasterRepositoryQuery.FindAll(q => q.InvoiceId > 0 && q.InvoiceTypeId == (int)DocumentType.itemsFund).ToList().Count();
                //if (codeExist != 0)
                //    NextCode = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == (int)DocumentType.itemsFund) + 1;
                var res = await generalProcess.SaveInvoice(parameter, setting, null,  (int)DocumentType.itemsFund, Aliases.InvoicesCode.ItemsFund , 0, FilesDirectories.ItemsFunds);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addItemsEntryFund);
                //GL integration
                await _iItemFundGLIntegrationService.AddItemFundJournalEntry(new ItemFundJournalEntryDTO()
                {
                    documentId = res.Id.Value,
                    totalAmount = parameter.InvoiceDetails.Sum(x=> x.Quantity*x.Price)
                    ,date=parameter.InvoiceDate
                });


                return res;
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Id = null, Result = Result.Failed };
            }
        }
    }
}
