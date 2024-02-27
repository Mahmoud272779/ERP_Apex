using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService
{
    public interface IAddTempInvoiceService
    {
        Task<ResponseResult> addTempInvoice(InvoiceMasterRequest parameter , int invoiceTypeId , string invoiceType);
        OfferPriceDetails ItemDetails(OfferPriceMaster invoice, InvoiceDetailsRequest item);
        List<InvoiceDetailsRequest> setCompositItem(OfferPriceMaster invoice, int itemId, int unitId, int indexOfItem, double qty);

    }
}
