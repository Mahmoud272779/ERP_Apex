using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ExtractPermission.UpdateExtractPermission
{
    public class UpdateExtractPermissionHandler : IRequestHandler<UpdateExtractPermissionRequest, ResponseResult>
    {
        private readonly IUpdateExtractPermissionService updateExtractPermissionServices;

        public UpdateExtractPermissionHandler(IUpdateExtractPermissionService updateExtractPermissionServices)
        {
            this.updateExtractPermissionServices = updateExtractPermissionServices;
        }

        public async Task<ResponseResult> Handle(UpdateExtractPermissionRequest request, CancellationToken cancellationToken)
        {
            return await updateExtractPermissionServices.UpdateExtractPermission(request);
        }
    }
}
