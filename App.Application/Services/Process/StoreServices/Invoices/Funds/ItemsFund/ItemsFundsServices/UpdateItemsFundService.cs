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
    public class UpdateItemsFundService : BaseClass, IUpdateItemsFundService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IUpdateInvoice GeneralProcessUpdate;
        private readonly iItemFundGLIntegrationService _iItemFundGLIntegrationService;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IHttpContextAccessor httpContext;
        public UpdateItemsFundService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                           IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IUpdateInvoice _GeneralProcessUpdate,
                              iItemFundGLIntegrationService  iItemFundGLIntegrationService,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              IGeneralAPIsService _GeneralAPIsService,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GeneralProcessUpdate = _GeneralProcessUpdate;
            _iItemFundGLIntegrationService = iItemFundGLIntegrationService;
            _systemHistoryLogsService = systemHistoryLogsService;
            generalAPIsService = _GeneralAPIsService;
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
        }
        public async Task<ResponseResult> UpdateItemsFund(UpdateInvoiceMasterRequest parameter)
        {
            parameter.InvoiceDate = generalAPIsService.serverDate(parameter.InvoiceDate);
            if (parameter.InvoiceDetails == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.ItemsIsReuired };

            }
            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);

            int NextCode = 1;
            if (InvoiceMasterRepositoryQuery.FindAll(q => q.InvoiceId > 0).Where(q => q.InvoiceTypeId == (int)DocumentType.itemsFund).Count() != 0)
                NextCode = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == (int)DocumentType.itemsFund) + 1;

            var res = await GeneralProcessUpdate.UpdateInvoices(parameter, setting, null, (int)DocumentType.itemsFund, Aliases.InvoicesCode.ItemsFund,FilesDirectories.ItemsFunds);
            await _iItemFundGLIntegrationService.AddItemFundJournalEntry(new ItemFundJournalEntryDTO()
            {
                documentId = parameter.InvoiceId,
                isUpdate = true,
                totalAmount = parameter.InvoiceDetails.Sum(x=> x.Quantity * x.Price),
                date = parameter.InvoiceDate,
            });
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editItemsEntryFund);
            return res;
        }

    }
}
