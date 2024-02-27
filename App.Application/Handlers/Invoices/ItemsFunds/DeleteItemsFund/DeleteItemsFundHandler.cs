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
    public class DeleteItemsFundHandler : IRequestHandler<DeleteItemsFundRequest, ResponseResult>
    {
        private readonly IDeleteItemsFundService DeleteItemfunds;

        public DeleteItemsFundHandler(IAddSalesService addSalesService, IDeleteItemsFundService deleteItemfunds)
        {
            DeleteItemfunds = deleteItemfunds;
        }

        public async Task<ResponseResult> Handle(DeleteItemsFundRequest request, CancellationToken cancellationToken)
        {
            return await DeleteItemfunds.DeleteItemsFund(request);
        }
    }
}
