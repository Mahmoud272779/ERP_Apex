using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.AdditionPermission.UpdateAdditionPermission
{
    public class UpdateAdditionPermissionHandler : IRequestHandler<UpdateAdditionPermissionRequest, ResponseResult>
    {
        private readonly IUpdateAdditionPermissionService updateAdditionPermissionService;

        public UpdateAdditionPermissionHandler(IUpdateAdditionPermissionService updateAdditionPermissionService)
        {
            this.updateAdditionPermissionService = updateAdditionPermissionService;
        }

        public async Task<ResponseResult> Handle(UpdateAdditionPermissionRequest request, CancellationToken cancellationToken)
        {
            return await updateAdditionPermissionService.UpdateAdditionPermission(request);
        }
    }
}
