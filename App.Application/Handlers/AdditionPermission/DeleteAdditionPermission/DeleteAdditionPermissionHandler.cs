using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.AdditionPermission.DeleteAdditionPermission
{
    public class DeleteAdditionPermissionHandler : IRequestHandler<DeleteAdditionPermissionRequest, ResponseResult>
    {
        private readonly IDeleteAdditionPermissionService deleteAdditionPermissionService;

        public DeleteAdditionPermissionHandler(IDeleteAdditionPermissionService deleteAdditionPermissionService)
        {
            this.deleteAdditionPermissionService = deleteAdditionPermissionService;
        }

        public async Task<ResponseResult> Handle(DeleteAdditionPermissionRequest request, CancellationToken cancellationToken)
        {
            return await deleteAdditionPermissionService.DeleteAdditionPermission(request);
        }
    }
}
