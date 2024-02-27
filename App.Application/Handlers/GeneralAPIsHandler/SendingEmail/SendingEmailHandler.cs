using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Services.HelperService.EmailServices;
using App.Application.Services.Printing.InvoicePrint;
using App.Domain.Entities;
using FastReport.Export.Html;
using FastReport.Export.PdfSimple;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.SendingEmail
{
    public class SendingEmailHandler : IRequestHandler<SendingEmailRequest, ResponseResult>
    {
        private readonly IEmailService _emailService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly IPrintService _printService;
        private readonly IFileHandler _fileHandler;

        public SendingEmailHandler(IEmailService emailService, IFilesMangerService filesMangerService, IPrintService printService, IFileHandler fileHandler)
        {
            _emailService = emailService;
            _filesMangerService = filesMangerService;
            _printService = printService;
            _fileHandler = fileHandler;
        }

        public async Task<ResponseResult> Handle(SendingEmailRequest parm, CancellationToken cancellationToken)
        {
            if (!Regex.IsMatch(parm.ToEmail, "^\\S+@\\S+\\.\\S+$"))
                return new ResponseResult()
                {
                    ErrorMessageAr = "البريد الالكتروني غير صحيح",
                    ErrorMessageEn = "Email is not correct",
                    Result = Result.Failed
                };

            if (parm.isInvoice)
            {
                var invoice = await GetInvoiceFile(new Domain.Models.Request.InvoiceDTO()
                {
                    exportType = exportType.ExportToPdf,
                    invoiceCode = parm.invoiceCode,
                    invoiceId = parm.invoiceId.Value,
                    isArabic = parm.isArabic,
                    screenId = parm.screenId.Value,
                });
                var ms = new MemoryStream(invoice);
                await ms.FlushAsync();

                parm.invoice = ms.ToArray();
            }

            var sendEmail = await _emailService.SendEmail(parm);

            return new ResponseResult()
            {
                Note = sendEmail == "OK" ? Actions.Success : Actions.SaveFailed,
                Result = sendEmail == "OK" ? Result.Success : Result.Failed
            };
        }

        WebReport webReport = new WebReport();
        private async Task<byte[]> GetInvoiceFile(App.Domain.Models.Request.InvoiceDTO invoiceDto)
        {

            var fileContents = await _filesMangerService.GetReportPrintFiles(invoiceDto.screenId, invoiceDto.isArabic);
            webReport = await _printService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId,"","", invoiceDto.exportType,invoiceDto.isArabic, isPriceOffer:(invoiceDto.screenId == (int)SubFormsIds.offerPrice_Sales ? true : false));

            webReport.Report.Prepare();
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.ImageDpi = 300;
            pdfExport.JpegQuality = 30;

            HTMLExport hTMLExport = new HTMLExport();
            hTMLExport.Print = true;
            hTMLExport.WidthUnits = HtmlSizeUnits.Percent;
            hTMLExport.Wysiwyg = true;
            hTMLExport.Pictures = true;
            hTMLExport.AllowOpenAfter = true;
            hTMLExport.EmbedPictures = true;
            hTMLExport.AllowOpenAfter = true;
            hTMLExport.EnableMargins = true;
            hTMLExport.ExtractMacros();

            MemoryStream ms = new MemoryStream();


            pdfExport.Export(webReport.Report, ms);
            ms.Flush();
            //var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
            //
            //var fileName = invoiceDto.invoiceCode + "-" + timenow;
            //var file = _fileHandler.CreateInvoiceForPrint(ms.ToArray(), fileName, "pdf");
            return ms.ToArray();
        }
    }
}
