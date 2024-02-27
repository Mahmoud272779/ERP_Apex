using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ItemsFunds.AddItemsFunds
{
    public class GetAllItemsFundsHandler : IRequestHandler<GetAllItemsFundsRequest, ResponseResult>
    {
        private readonly IGetAllitemsFundService GetAllitemsFund;

        public GetAllItemsFundsHandler(IGetAllitemsFundService getAllitemsFund)
        {
            GetAllitemsFund = getAllitemsFund;
        }

        public async Task<ResponseResult> Handle(GetAllItemsFundsRequest request, CancellationToken cancellationToken)
        {
            return await GetAllitemsFund.GetAllItemsFund(request);
        }
    }
}
