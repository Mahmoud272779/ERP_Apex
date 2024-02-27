using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ExtractPermission
{
    public class AddExtractPermissionHandler : IRequestHandler<AddExtractPermissionRequest, ResponseResult>
    {
        private readonly IAddExtractPermissionService addExtractPermissionService;

        public AddExtractPermissionHandler(IAddExtractPermissionService addExtractPermissionService)
        {
            this.addExtractPermissionService = addExtractPermissionService;
        }

        public async Task<ResponseResult> Handle(AddExtractPermissionRequest request, CancellationToken cancellationToken)
        {
            return await addExtractPermissionService.AddExtractPermission(request);
        }
    }
}
