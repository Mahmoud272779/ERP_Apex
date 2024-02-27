using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.TransferToSales
{
    public class TransferToSalesHandler:IRequestHandler<TransferToSalesRequest,ResponseResult>
    {
        private readonly ITransferToSalesService transferToSalesService;
        public TransferToSalesHandler(ITransferToSalesService transferToSalesService)
        {
            this.transferToSalesService = transferToSalesService;
        }
        public async Task<ResponseResult> Handle(TransferToSalesRequest request , CancellationToken cancellationToken)
        {
            return await  transferToSalesService.transferTosales(request.Id);
        }
    }
}
