using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
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
    public class AddExtractPermissionService : BaseClass, IAddExtractPermissionService
    {
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;

        private IAddInvoice generalProcess;

        public AddExtractPermissionService(
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;

            this.generalProcess = generalProcess;
        }

        public async Task<ResponseResult> AddExtractPermission(InvoiceMasterRequest parameter)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                if (parameter.StoreId == 0)
                { return new ResponseResult() {
                        Data = null,Id = null, Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.SelectStore,  ErrorMessageEn = ErrorMessagesEn.SelectStore};
                }
                if (parameter.InvoiceDetails == null)
                { return new ResponseResult(){
                        Data = null,Id = null,Result = Result.RequiredData,ErrorMessageAr = ErrorMessagesAr.NoItems,ErrorMessageEn = ErrorMessagesEn.NoItems };
                }
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

               var res = await generalProcess.SaveInvoice(parameter, setting, null, (int)DocumentType.ExtractPermission, Aliases.InvoicesCode.ExtractPermission, 0, null);

                return res;
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Id = null, Result = Result.Failed };
            }
        }

    }
}
