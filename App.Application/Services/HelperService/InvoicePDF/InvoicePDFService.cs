using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Enums;
using App.Domain.Models.Request;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.InvoicePDF
{
    public class InvoicePDFService : iInvoicePDFService
    {
        private readonly IPrintService _iPrintService;
        private readonly IFilesMangerService _filesMangerService;

        public InvoicePDFService(/*IPrintService iPrintService, IFilesMangerService filesMangerService*/)
        {
            //_iPrintService = iPrintService;
            //_filesMangerService = filesMangerService;
        }
        WebReport webReport = new WebReport();
        public async Task<Tuple<string, byte[]>> getInvoicePDF(InvoiceDTO invoiceDto)
        {
            var fileContents = await _filesMangerService.GetReportPrintFiles(invoiceDto.screenId, invoiceDto.isArabic);
           // exportType type = new exportType();
           
            webReport = await _iPrintService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId,"","", invoiceDto.exportType,invoiceDto.isArabic);

            //webReport.Report.Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();

            MemoryStream ms = new MemoryStream();


            pdfExport.Export(webReport.Report, ms);
            var path = "";
            List<string> PathForDelete = new List<string>()
            {
                path
            };
            ms.Flush();
            return new Tuple<string, byte[]>(invoiceDto.invoiceCode,ms.ToArray());

        }
    }
}
