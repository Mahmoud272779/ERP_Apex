using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process
{
    public  interface IGetAllExtractPermissionService
    {
        Task<ResponseResult> GetAllExtractPermission(StoreSearchPagination parameter);

    }
}
