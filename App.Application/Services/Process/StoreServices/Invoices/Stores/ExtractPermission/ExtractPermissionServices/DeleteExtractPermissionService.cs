using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class DeleteExtractPermissionService : BaseClass, IDeleteExtractPermissionService
    {

        private readonly IDeleteInvoice GeneralProcessDelete;
        private readonly IHttpContextAccessor httpContext;

        public DeleteExtractPermissionService( IDeleteInvoice generalProcessDelete, 
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            httpContext = _httpContext;
            GeneralProcessDelete = generalProcessDelete;
        }
        public async Task<ResponseResult> DeleteExtractPermission(SharedRequestDTOs.Delete parameter)
        {
            var res = await GeneralProcessDelete.DeleteInvoices(parameter.Ids.First(), (int)DocumentType.DeleteExtractPermission, Aliases.InvoicesCode.DeleteExtractPermission, (int)DocumentType.ExtractPermission);
            return res;
        }


    }
}
