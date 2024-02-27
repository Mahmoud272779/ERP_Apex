using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.sales.GetAllSales
{
    public class GetAllOfferPriceHandler : IRequestHandler<GetAllOfferPriceRequest, ResponseResult>
    {
        private readonly IGetAllTempInvoicesServices getAllTempInvoicesServices;

        public GetAllOfferPriceHandler(IGetAllTempInvoicesServices getAllTempInvoicesServices)
        {
            this.getAllTempInvoicesServices = getAllTempInvoicesServices;
        }

        public async Task<ResponseResult> Handle(GetAllOfferPriceRequest request, CancellationToken cancellationToken)
        {
            return await getAllTempInvoicesServices.GetAllTempInvoices(request,(int)DocumentType.OfferPrice,(int)DocumentType.DeleteOfferPrice);
        }
    }
}
