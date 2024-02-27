using App.Application.Services.Process;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.ExtractPermission
{
    public class GetAllExtractPermissionHandler : IRequestHandler<GetAllExtractPermissionReqeust, ResponseResult>
    {
        private readonly IGetAllExtractPermissionService getAllExtractPermissionServices;

        public GetAllExtractPermissionHandler(IGetAllExtractPermissionService getAllExtractPermissionServices)
        {
            this.getAllExtractPermissionServices = getAllExtractPermissionServices;
        }

        public async Task<ResponseResult> Handle(GetAllExtractPermissionReqeust request, CancellationToken cancellationToken)
        {
            return await getAllExtractPermissionServices.GetAllExtractPermission(request);
        }
    }
}
