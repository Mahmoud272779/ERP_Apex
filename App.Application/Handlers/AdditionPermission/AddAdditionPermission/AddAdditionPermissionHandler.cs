using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.AdditionPermission.AddAdditionPermission
{
    public class AddAdditionPermissionHandler : IRequestHandler<AddAdditionPermissionRequest, ResponseResult>
    {
        private readonly IAddAdditionPermissionService addAdditionPermissionService;

        public AddAdditionPermissionHandler(IAddAdditionPermissionService addAdditionPermissionService)
        {
            this.addAdditionPermissionService = addAdditionPermissionService;
        }

        public async Task<ResponseResult> Handle(AddAdditionPermissionRequest request, CancellationToken cancellationToken)
        {
            return await addAdditionPermissionService.AddAdditionPermission(request);
        }
    }
}
