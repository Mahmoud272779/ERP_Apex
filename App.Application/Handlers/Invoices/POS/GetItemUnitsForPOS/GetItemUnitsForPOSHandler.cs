using App.Application.Services.Process.StoreServices.Invoices.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetItemUnitsForPOS
{
    public class GetItemUnitsForPOSHandler : IRequestHandler<GetItemUnitsForPOSRequest, ResponseResult>
    {
        private readonly IPosService posService;

        public GetItemUnitsForPOSHandler(IPosService posService)
        {
            this.posService = posService;
        }

        public async Task<ResponseResult> Handle(GetItemUnitsForPOSRequest request, CancellationToken cancellationToken)
        {
            return await posService.getItemUnitsForPOS(request.itemId);
        }
    }
}
