using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
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

namespace App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.AdditionPermissionServices
{
    public class AddAdditionPermissionService : BaseClass, IAddAdditionPermissionService
    {

        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
     
        private IAddInvoice generalProcess;

        public AddAdditionPermissionService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;

            this.generalProcess = generalProcess;
        }

        public async Task<ResponseResult> AddAdditionPermission(InvoiceMasterRequest parameter)
        {
            try
            {
                parameter.ParentInvoiceCode = "";
                if (parameter.InvoiceDetails == null)
                {
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "you should send at least one item" };

                }
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

                // validation of total result
                //int NextCode = 1;
                //var codeExist = InvoiceMasterRepositoryQuery.FindAll(q => q.InvoiceId > 0 && q.InvoiceTypeId == (int)DocumentType.AddPermission).ToList().Count();
                //if (codeExist != 0)
                //    NextCode = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == (int)DocumentType.AddPermission) + 1;
                var res = await generalProcess.SaveInvoice(parameter, setting,null, (int)DocumentType.AddPermission, Aliases.InvoicesCode.AddPermission, 0, FilesDirectories.AdditionPermission);

                return res;
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Id = null, Result = Result.Failed };
            }
        }
    }
}
