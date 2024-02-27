using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService
{
    public  interface IDeleteTempInvoiceService
    {
        Task<ResponseResult> DeleteTempInvoice(SharedRequestDTOs.Delete parameter, int invoiceTypeId, string invoiceType , int mainInvoiceTypeId);
    }
}
