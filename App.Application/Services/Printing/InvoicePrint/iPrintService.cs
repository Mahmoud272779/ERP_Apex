
using App.Domain.Enums;
using FastReport.Web;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;

namespace App.Application.Services.Printing.InvoicePrint
{
    public interface IPrintService
    {
        //Task<WebReport> Report(List<DataTable> data, byte[] fileContents, exportType type);
        // Task<WebReport> ReportInvoice(int invoicedId, byte[] file,string ReportName);
        Task<WebReport> ReportInvoice(byte[] fileContents, int invoiceId, string employeeNameAr, string employeeNameEn, exportType type, bool isArabic, bool isPOS = false, bool isPriceOffer = false);

        void SetPrintToInvoiceHistory(InvoiceDto invoiceDto, exportType type);
        void RoundedNumbers(ref InvoiceDto invoiceDto);
        void CheckPrintSerials(ref InvoiceDto invoiceDto);

    }
}
