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
    public class AddItemsFundsHandler : IRequestHandler<AddItemsFundsRequest, ResponseResult>
    {
        private readonly IAddItemsFundService addItemsFundsService;

        public AddItemsFundsHandler(IAddItemsFundService addItemFundsService)
        {
            this.addItemsFundsService = addItemFundsService;
        }

        public async Task<ResponseResult> Handle(AddItemsFundsRequest request, CancellationToken cancellationToken)
        {
            return await addItemsFundsService.AddItemsFund(request);
        }
    }
}
