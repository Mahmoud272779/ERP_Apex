using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;

namespace App.Application.Handlers.ExtractPermission
{
    public class UpdateExtractPermissionRequest : UpdateInvoiceMasterRequest,IRequest<ResponseResult>
    {
    }
}
