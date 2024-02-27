using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.UpdateOfferPriceStatus
{
    public class UpdateOfferPriceStatusHandler : IRequestHandler<UpdateOfferPriceStatusRequest, ResponseResult>
    {
        private readonly ITransferToSalesService transferToSalesService;

        public UpdateOfferPriceStatusHandler(ITransferToSalesService transferToSalesService)
        {
            this.transferToSalesService = transferToSalesService;
        }
        public async Task<ResponseResult> Handle(UpdateOfferPriceStatusRequest request , CancellationToken cancellationToken)
        {
            return await transferToSalesService.updateStatus(request.offerPriceId);

        }
    }
}
