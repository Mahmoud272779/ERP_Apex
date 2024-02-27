using App.Application.Handlers.Purchases.AddPurchases;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice
{
    public  class AddOfferPriceHandler : IRequestHandler<AddOfferPriceRequest, ResponseResult>
    {
        private readonly IAddTempInvoiceService OfferPriceService;


        public AddOfferPriceHandler(IAddTempInvoiceService OfferPriceService)
        {

            this.OfferPriceService = OfferPriceService;
        }

        public async Task<ResponseResult> Handle(AddOfferPriceRequest request, CancellationToken cancellationToken)
        {

            return await OfferPriceService.addTempInvoice(request ,(int)DocumentType.OfferPrice, InvoicesCode.OfferPrice);
        }
    }
}
