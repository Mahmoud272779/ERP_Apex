using App.Application.Services.Process.FileManger.ReportFileServices;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Shared;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport.Export.PdfSimple;
using FastReport.Web;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Neodynamic.SDK.Web;
using App.Domain.Models.Request;
using App.Application.Services.Printing;
using Hangfire;
using App.Application.Helpers.Service_helper.FileHandler;
using Microsoft.Extensions.Configuration;
using App.Domain.Models.Response.General;
using Microsoft.AspNetCore.Hosting;
using App.Domain.Enums;
using App.Application.Services.HelperService.authorizationServices;
using static App.Domain.Enums.Enums;
using App.Domain.Models.Security.Authentication.Response;
using System.Runtime;
using FastReport.Export.Html;
using App.Application.Helpers;
using Microsoft.Net.Http.Headers;
using FastReport.Data;
using App.Application.Services.Printing.OfferPriceLanding;
using System.Data;
using System.Reflection;
using App.Application.Helpers.UpdateSystem.Updates;
using App.Application.Services.Printing.UpdatePrintFiles;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Printing.InvoicePrint;

namespace App.Api.Controllers.Process.GeneralLedger.ReportsController
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    // [ApiController]
    public class InvioceReportController : Controller
    {
        
        private readonly IPrintService iPrintService;
        private readonly IReportFileService _ireportFileService;
        private IFilesMangerService _filesMangerService;
        private readonly IFileHandler _fileHandler;
        private readonly IConfiguration _configuration;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IprintFileService _printFileService;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        private readonly IHttpContextAccessor httpContext;
        private readonly IOfferPriceLanding _iOfferPriceLanding;
        private readonly IUpdatePrintFiles _iUpdatePrintFiles;
        public InvioceReportController(IPrintService iPrintService,
        IReportFileService ireportFileService, IFilesMangerService filesMangerService,
        IFileHandler fileHandler, IConfiguration configuration, iAuthorizationService iAuthorizationService,
        IWebHostEnvironment webHostEnvironment, IprintFileService printFileService, iUserInformation iUserInformation, IPermissionsForPrint iPermmissionsForPrint, IHttpContextAccessor httpContext, IOfferPriceLanding iOfferPriceLanding, IUpdatePrintFiles iUpdatePrintFiles)
        {
            this.iPrintService = iPrintService;
            _ireportFileService = ireportFileService;
            _filesMangerService = filesMangerService;
            _fileHandler = fileHandler;
            _configuration = configuration;
            _iAuthorizationService = iAuthorizationService;
            _webHostEnvironment = webHostEnvironment;
            _printFileService = printFileService;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
            this.httpContext = httpContext;
            _iOfferPriceLanding = iOfferPriceLanding;
            _iUpdatePrintFiles = iUpdatePrintFiles;
        }
        WebReport webReport = new WebReport();

        [HttpGet("InvoiceReport")]
        public async Task<IActionResult> InvoiceReport(InvoiceDTO invoiceDto, int invoiceTypeId)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, invoiceDto.screenId);
            _ResponseResult res = new _ResponseResult();


            if (permissions.IsPrint)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized(0, invoiceDto.screenId, Opretion.Print);
                if (isAuthorized != null) return Ok(isAuthorized);
                bool isPos = false;
                if (invoiceTypeId == 11 || invoiceTypeId == 12)//POS OR ReturnPOS
                {
                    isPos = true;
                }
                else if (invoiceTypeId == 38 || invoiceTypeId == 40)// Price Offer
                {
                    invoiceDto.isPriceOffer = true;
                }
                var file = await _filesMangerService.GetInviocePrintFile(invoiceDto,userInfo.employeeNameAr.ToString(),userInfo.employeeNameEn.ToString(), isPos);
                res.result = file;
                res.result.ResultForPrint = Result.Success;
            }
            else
            {
                var reportsReponse = new ReportsReponse()
                {
                    Result = Result.UnAuthorized

                };
                res.result = reportsReponse;
            }
            return Ok(res);

            #region commented
            //webReport.Report.Prepare();

            //PDFSimpleExport pdfExport = new PDFSimpleExport();

            //MemoryStream ms = new MemoryStream();


            //pdfExport.Export(webReport.Report, ms);
            //var path = "";
            //List<string> PathForDelete = new List<string>()
            //{
            //    path
            //};
            //ms.Flush();

            //var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
            //var fileName = invoiceDto.invoiceCode + "-" + timenow;
            //var file = _fileHandler.CreateInvoiceForPrint(ms.ToArray(), fileName, "pdf");
            //BackgroundJob.Schedule(() => _fileHandler.DeleteImage(file.Item2), TimeSpan.FromMinutes(10));
            //return Ok(new ReportsReponse()
            //{
            //    FileURL = file.Item1,
            //    FileName = file.Item3,
            //    Result = !string.IsNullOrEmpty(file.Item3) ? Domain.Enums.Enums.Result.Success : Domain.Enums.Enums.Result.Failed
            //});
            // var fileData =  _printFileService.PrintFile(webReport, SavedFileName: invoiceDto.invoiceCode);
            //return Ok( new ReportsReponse()
            //{
            //    FileURL = fileData.FileURL,
            //    FileName = fileData.FileName,
            //    Result = fileData.Result
            //}) ;
            //return Ok(fileData);
            //return Ok(fileData);

            //PDFSimpleExport pdfExport = new PDFSimpleExport();

            //MemoryStream ms = new MemoryStream();


            //pdfExport.Export(webReport.Report, ms);

            //ms.Flush();

            //var path = "";
            //List<string> PathForDelete = new List<string>()
            //{
            //    path
            //};
            //var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
            //var fileName = invoiceDto.invoiceCode + "-" + timenow;
            //var file = _fileHandler.CreateInvoiceForPrint(ms.ToArray(), fileName, "pdf");
            //BackgroundJob.Schedule(() => _fileHandler.DeleteImage(file.Item2), TimeSpan.FromMinutes(10));
            //return Ok(new ReportsReponse()
            //{
            //    FileURL = file.Item1,
            //    FileName = file.Item3,
            //    Result = !string.IsNullOrEmpty(file.Item3) ? Domain.Enums.Enums.Result.Success : Domain.Enums.Enums.Result.Failed
            //});



            //var fileContents = await _filesMangerService.GetPrintFiles(invoiceDto.screenId, invoiceDto.isArabic);
            //webReport = await iPrintService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId);
            ////return  webReport.PrintHtml();
            //ViewBag.WebReport = webReport;
            //// return webReport.PrintHtml();
            //return View();
            #endregion
        }
        //private async Task<ReportsReponse> GetPrintFile(InvoiceDTO invoiceDto)
        //{
        //    var fileContents = await _filesMangerService.GetPrintFiles(invoiceDto.screenId, invoiceDto.isArabic);
        //    webReport = await iPrintService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId, invoiceDto.exportType);
        //    return await _printFileService.PrintFile(webReport, invoiceDto.invoiceCode);
        //}
        [HttpGet("ExportToPdf")]
        public async Task<IActionResult> ExportToPdf(InvoiceDTO invoiceDto, exportType type)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0, invoiceDto.screenId, Opretion.Print);
            if (isAuthorized != null) return Ok(isAuthorized);

           
            var ms = await GetReportMS(invoiceDto,type);

            //return File(ms, "application/pdf", "Invoice.pdf");
            return Ok();

        }
        [HttpGet("GetReportMS")]
        public  async Task<IActionResult> GetReportMS(InvoiceDTO invoiceDto, exportType type)
        {
            var fileContents = await _filesMangerService.GetReportPrintFiles(invoiceDto.screenId, invoiceDto.isArabic);

            //ViewBag.WebReport = await iPrintService.ReportInvoice(fileContents.Files, invoiceDto.invoiceId, type,true);
            //HTMLExport html = new HTMLExport();
            //webReport.Report.Export(html, "report.html");
            //webReport.Report.Prepare();
            //ViewBag.WebReport=
          

            //PDFSimpleExport pdfExport = new PDFSimpleExport();

            //MemoryStream ms = new MemoryStream();

            //var str = webReport.Report.SaveToStringBase64();
            //var str1 = webReport.Report.SaveToString();
           
            //var bytes = Convert.FromBase64String(str);
            //pdfExport.Export(webReport.Report, ms);
            //ms.Flush();
            //return ms;
            return Ok();
        }
        
        [HttpPost("DirectPrint")]
        public async Task<IActionResult> DirectPrint([FromQuery] DirectPrintRequstDTO parm,bool isPOS)
        {

           // var isAuthorized = await _iAuthorizationService.isAuthorized(0, parm.invoiceDto.screenId, Opretion.Print);
           // if (isAuthorized != null) return Ok(isAuthorized);
           // var file = await _filesMangerService.GetPrintFile(parm.invoiceDto, isPOS);
           // var filePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], file.FileName);
           // var fileAsBytes = System.IO.File.ReadAllBytes(filePath);
           // PdfDocument pdfDocument = new PdfDocument(fileAsBytes);
           //// pdfDocument.GetPrintDocument().PrinterSettings.
           // pdfDocument.Print(parm.printerName);
           // return Ok();
            //httpContext.HttpContext.Request.Query[HeaderNames.UserAgent].ToString(),
            //string printers = httpContext.HttpContext.Request.Query["printers"].ToString();
            //printers = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(printers));
            ////foreach (string sPrinters in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            ////{
            ////    printers.Add(sPrinters);
            ////}
            //return Ok(printers);
            return Ok();
        }
        #region commeneted
        [HttpGet("ExportToExcel")]
        //public async Task<ActionResult> ExportToExcel(InvoiceDTO invoiceDto)
        //{

           
           
        //    // ex.
        //    // var file = await ExportToPdf(invoiceDto);
        //    byte[] file=null;
        //    Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument(file);

        //    MemoryStream ms = new MemoryStream();
        //    document.SaveToStream(ms, Spire.Pdf.FileFormat.XLSX);

        //    var timenow = DateTime.Now.ToString().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
        //    var fileName = invoiceDto.invoiceCode + "-" + timenow;
        //    var _file = _fileHandler.CreateInvoiceForPrint(ms.ToArray(), fileName, "xlsx");
        //    BackgroundJob.Schedule(() => _fileHandler.DeleteImage(_file.Item2), TimeSpan.FromMinutes(10));
        //    return Ok(new ReportsReponse()
        //    {
        //        FileURL = _file.Item1,
        //        FileName = _file.Item3,
        //        Result = !string.IsNullOrEmpty(_file.Item3) ? Domain.Enums.Enums.Result.Success : Domain.Enums.Enums.Result.Failed
        //    });
        //    //return File(ms.ToArray(), "application/vnd.ms-excel", "Invoice.xls");
        //}
        #endregion
        [HttpGet("GetScreenFiles")]
        public async Task<Domain.Models.Shared.ResponseResult> GetScreenFiles( int screenId)
        {

            return await _filesMangerService.GetAllPrintFiles(screenId);

        }
        [HttpGet("GetFileById")]
        public async Task<Domain.Models.Shared.ResponseResult> GetFileById(int fileId)
        {

            return await _filesMangerService.GetPrintFileById(fileId);

        }

        [HttpPost("UpdateInvoiceFile")]
        public async Task<Domain.Models.Shared.ResponseResult> UpdateInvoiceFile(ReportFileRequest reportFileRequest)
        {
            reportFileRequest.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, reportFileRequest.ReportFileName, reportFileRequest.IsArabic);
            return await _ireportFileService.UpdateReport(reportFileRequest);

        }
        [HttpGet("OfferPriceLandingPrint")]
        public async Task<IActionResult> OfferPriceLandingPrint()
        {

            

            var report= await _iOfferPriceLanding.OfferPriceLandingPrint();
            return Ok( _printFileService.PrintFile(report, "OfferPriceLanding", exportType.Print));
            //return Ok(report);

        }

        [HttpGet("UpdatePrintFiles")]
        public async Task<ResponseResult> UpdatePrintFiles()
        {

          return await _iUpdatePrintFiles.UpdatePrintFiles();

            
            //return Ok(report);

        }

        


        // [HttpPost("ExporDataToFile")]
        //public async Task<IActionResult> ExporDataToFile(int invoiceId, int screenId, bool isArabic)
        //{

        //    var dictioneryexportType = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
        //    var exportType = dictioneryexportType["Export"];

        //    switch (exportType)
        //    {

        //        case "Pdf":
        //            return await ExportToPdf(invoiceId,screenId,isArabic,false);
        //            break;
        //        case "Excel":
        //            return await ExportToExcel(invoiceId,screenId,isArabic);
        //            break;

        //    }
        //    return null;

        //}
        //[HttpPost("PrintReport")]
        //[AllowAnonymous]
        //public async Task<IActionResult> PrintReport(int invoiceId)
        //{

        //    // int  invoiceId = 3476;

        //    //var file = await ExportToPdf(invoiceId, 3, true, true);

        //    var pdfPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Invoice-62636.pdf");
        //    var file = System.IO.File.ReadAllBytes(pdfPath);
        //    PdfDocument pdfDocument = new PdfDocument(file);
        //    // var path = Path.Combine("wwwroot", "App_Data", "Invoice" + ".pdf");

        //    // var pdf=pdfDocument.GetPrintDocument();
        //    //LocalPrintServer ps = new LocalPrintServer();
        //    //PrintQueue pq = ps.DefaultPrintQueue;
        //    return Ok(pdfDocument.Print("Microsoft Print to PDF"));
        //    //// Get an XpsDocumentWriter for the default print queue
        //    //XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(pq);
        //    //PrintJob printJob = new PrintJob(pdfDocument.GetPrintDocument());
        //    //printJob.Print();
        //    // ClientPrintJob cpj = new ClientPrintJob();
        //    //// byte[] bytes= 

        //    // //use default printer on the client machine
        //    // cpj.ClientPrinter = new DefaultPrinter();
        //    // PrintFile printerFile = new PrintFile(pdfDocument.BinaryData, "invoice");
        //    //set the commands to send to the printer
        //    //cpj.PrinterCommands = "PRINTER_COMMANDS_GO_HERE";



        //    //  pdfDocument.GetPrintDocument().PrinterSettings.IsDefaultPrinter;
        //    // pdfDocument.GetPrintDocument().PrinterSettings.DefaultPageSettings.;
        //    //  return Ok(pdfDocument.Print());
        //    //  return Ok(pdfDocument);

        //}


        [HttpPost("SaveReport")]
        public async Task<Domain.Models.Shared.ResponseResult> SaveReport(ReportFileRequest reportFileRequest)
        {

            reportFileRequest.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, reportFileRequest.ReportFileName, reportFileRequest.IsArabic);

            return await _ireportFileService.SaveReort(reportFileRequest);

        }


    }
}
