using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.AdditionPermission.GetAdditionPermissionById
{
    public class GetAdditionPermissionByIdHandler : IRequestHandler<GetAdditionPermissionByIdRequest, ResponseResult>
    {
        private readonly IGetInvoiceByIdService GetAddPermissionServiceByIdService;

        public GetAdditionPermissionByIdHandler(IGetInvoiceByIdService getAddPermissionServiceByIdService)
        {
            GetAddPermissionServiceByIdService = getAddPermissionServiceByIdService;
        }

        public async Task<ResponseResult> Handle(GetAdditionPermissionByIdRequest request, CancellationToken cancellationToken)
        {
            return await GetAddPermissionServiceByIdService.GetInvoiceById(request.InvoiceId, request.isCopy);
        }
    }
}
