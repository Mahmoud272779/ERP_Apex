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
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.AdditionPermissionServices
{
    public class UpdateAdditionPermissionService : BaseClass, IUpdateAdditionPermissionService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IUpdateInvoice GeneralProcessUpdate;
        private readonly IHttpContextAccessor httpContext;
        public UpdateAdditionPermissionService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                           IRepositoryQuery<InvGeneralSettings> generalSettings,
                              IUpdateInvoice _GeneralProcessUpdate,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GeneralProcessUpdate = _GeneralProcessUpdate;
            httpContext = _httpContext;
            GeneralSettings = generalSettings;
        }
        public async Task<ResponseResult> UpdateAdditionPermission(UpdateInvoiceMasterRequest parameter)
        {
            if (parameter.InvoiceDetails == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.ItemsIsReuired };

            }
            if (parameter.InvoiceId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);

            int NextCode = 1;
            if (InvoiceMasterRepositoryQuery.FindAll(q => q.InvoiceId > 0).Where(q => q.InvoiceTypeId == (int)DocumentType.AddPermission).Count() != 0)
                NextCode = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == (int)DocumentType.AddPermission) + 1;
        
            var res = await GeneralProcessUpdate.UpdateInvoices(parameter, setting, null, (int)DocumentType.AddPermission, Aliases.InvoicesCode.AddPermission,FilesDirectories.AdditionPermission);
            return res;
        }

    }
}
