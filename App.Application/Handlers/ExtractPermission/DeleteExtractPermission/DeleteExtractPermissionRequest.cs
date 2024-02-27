using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ExtractPermission.DeleteExtractPermission
{
    public class DeleteExtractPermissionRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
