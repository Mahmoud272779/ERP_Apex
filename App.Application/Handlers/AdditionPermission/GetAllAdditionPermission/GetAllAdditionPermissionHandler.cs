using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.AdditionPermission.GetAllAdditionPermission
{
    public class GetAllAdditionPermissionHandler : IRequestHandler<GetAllAdditionPermissionRequest, ResponseResult>
    {
        private readonly IGetAllAdditionPermissionService GetAllAdditionPermissionService;

        public GetAllAdditionPermissionHandler(IGetAllAdditionPermissionService getAllAdditionPermissionService)
        {
            GetAllAdditionPermissionService = getAllAdditionPermissionService;
        }

        public async Task<ResponseResult> Handle(GetAllAdditionPermissionRequest request, CancellationToken cancellationToken)
        {
            return await GetAllAdditionPermissionService.GetAllAdditionPermission(request);
        }
    }
}
