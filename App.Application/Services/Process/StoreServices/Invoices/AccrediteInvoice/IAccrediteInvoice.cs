using App.Domain.Models.Request.Store.Invoices;
using App.Domain.Models.Shared;
using System;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice
{
    public interface IAccrediteInvoice
    {
        Task<ResponseResult> GetAccrediteInvoiceData(AccreditInvoiceRequest parameter, bool isPrint = false);
        Task<ResponseResult> GetAccrediteInvoicPaymentTypeData(AccreditInvoiceRequest parameter, bool isPrint = false);
        Task<ResponseResult> GetAccrediteInvoicSafeBankData(AccreditInvoiceRequest parameter);
        Task<ResponseResult> accrediteAllInvoice(AccreditInvoiceRequest parameter);
        Task<ResponseResult> setAccreditReceipts(AccreditInvoiceRequest parameter);
        Task<ResponseResult> GetAccrediteInvoiceDataCount(AccreditInvoiceRequest parameter);
        Task<ResponseResult> AccrediteInvoiceLast(AccreditInvoiceRequest parameter);
        Task<WebReport> GetAccrediteInvoiceDataReport(AccreditInvoiceRequest parameter, InvoiceClosing invoiceClosing, int screenId, exportType exportType, bool isArabic, int fileId = 0);
        

        //public byte[] ReadAllBytes(string fileName);

    }
}
