using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.GeneralAPIsHandler.SendingEmail;
using App.Application.Services.HelperService.EmailServices;
using App.Application.Services.Printing;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Enums;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    public class EmailServicesController : ApiStoreControllerBase
    {
        //private readonly IGeneralAPIsService _generalAPIsService;
        //private readonly IprintFileService _printFileService;
        //private readonly IPrintService _iPrintService;
        //private readonly IConfiguration _configuration;
        //private IFilesMangerService _filesMangerService;
        private IMediator _mediator;

        public EmailServicesController(IActionResultResponseHandler ResponseHandler, IMediator mediator) : base(ResponseHandler)
        {
            _mediator = mediator;
        }



        //public EmailServicesController(
        //                IGeneralAPIsService generalAPIsService,

        //                IActionResultResponseHandler ResponseHandler,
        //                IprintFileService printFileService,
        //                IPrintService iPrintService,
        //                Microsoft.Extensions.Configuration.IConfiguration configuration,
        //                IFilesMangerService filesMangerService,
        //                IMediator mediator) : base(ResponseHandler)
        //{
        //    _generalAPIsService = generalAPIsService;
        //    _printFileService = printFileService;
        //    _iPrintService = iPrintService;
        //    _configuration = configuration;
        //    _filesMangerService = filesMangerService;
        //    _mediator = mediator;
        //}
        [HttpPost("InvoiceEmailSender")]
        public async Task<IActionResult> InvoiceEmailSender([FromForm] SendingEmailRequest parm)
        {

            //var fileContents = await _filesMangerService.GetPrintFiles(parm.screenId??0, parm.isArabic);
            ////WebReport webReport = await _iPrintService.ReportInvoice(fileContents.Files, parm.invoiceId??0, exportType.ExportToPdf);
            //WebReport webReport = new WebReport();
            //var report = await _printFileService.PrintFile(webReport, parm.invoiceCode, exportType.ExportToPdf);
            //byte[] file = null;
            //var reportPath = System.IO.Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], report.FileName);
            //if (System.IO.File.Exists(reportPath))
            //    file = System.IO.File.ReadAllBytes(reportPath);
            //if (file != null)
            //{
            //    parm.isInvoice = true;
            //    parm.invoice = file;
            //}
            //if (parm.subject == null || parm.subject == string.Empty)
            //{
            //    parm.subject = parm.invoiceCode;
            //}
            parm.isInvoice = true;
            parm.subject = parm.invoiceCode;
            var emailSender = await _mediator.Send(parm);
            return Ok(emailSender);
        }


    }
}
