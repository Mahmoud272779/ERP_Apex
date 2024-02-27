using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
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

namespace App.Application
{
    public class UpdateExtractPermissionService:BaseClass, IUpdateExtractPermissionService
    {
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IUpdateInvoice GeneralProcessUpdate;
        private readonly IHttpContextAccessor httpContext;
        public UpdateExtractPermissionService(IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IUpdateInvoice _GeneralProcessUpdate,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            GeneralProcessUpdate = _GeneralProcessUpdate;
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
        }
        public async Task<ResponseResult> UpdateExtractPermission(UpdateInvoiceMasterRequest parameter)
        {

            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);

            var res = await GeneralProcessUpdate.UpdateInvoices(parameter, setting, null, (int)DocumentType.ExtractPermission, Aliases.InvoicesCode.ExtractPermission , "");
            return res;
        }
    }
}
