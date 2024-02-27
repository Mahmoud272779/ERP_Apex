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
    public class UpdateItemsFundHandler : IRequestHandler<UpdateItemsFundRequest, ResponseResult>
    {
        private readonly IUpdateItemsFundService updateItemsFund;

        public UpdateItemsFundHandler(IAddSalesService addSalesService, IUpdateItemsFundService updateItemsFund)
        {
            this.updateItemsFund = updateItemsFund;
        }

        public async Task<ResponseResult> Handle(UpdateItemsFundRequest request, CancellationToken cancellationToken)
        {
            return await updateItemsFund.UpdateItemsFund(request);
        }
    }
}
