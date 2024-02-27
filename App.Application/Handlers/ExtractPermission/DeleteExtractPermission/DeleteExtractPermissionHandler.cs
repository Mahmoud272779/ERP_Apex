using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ExtractPermission.DeleteExtractPermission
{
    public class DeleteExtractPermissionHandler : IRequestHandler<DeleteExtractPermissionRequest, ResponseResult>
    {
        private readonly IDeleteExtractPermissionService deleteExtractPermissionServices;

        public DeleteExtractPermissionHandler(IDeleteExtractPermissionService deleteExtractPermissionServices)
        {
            this.deleteExtractPermissionServices = deleteExtractPermissionServices;
        }

        public async Task<ResponseResult> Handle(DeleteExtractPermissionRequest request, CancellationToken cancellationToken)
        {
            return await deleteExtractPermissionServices.DeleteExtractPermission(request);
        }
    }
}
