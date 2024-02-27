using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ExtractPermission
{
    public class GetAllExtractPermissionReqeust : StoreSearchPagination,IRequest<ResponseResult>
    {
    }
}
