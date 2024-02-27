using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Api.Controllers.Process.GeneralLedger.ReportsController;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.Color;
using App.Application.Services.Process.FileManger.ReportFileServices;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Process
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FileMangerController : Controller
    {
      
      


        private readonly IFilesMangerService FilemangerService;
        public FileMangerController(IFilesMangerService filemangerService,
                        IActionResultResponseHandler ResponseHandler ,
                        IPrintService iPrintService,
                        IReportFileService ireportFileService
                        ) 
        {
            FilemangerService = filemangerService;
         
          
        }



       // InvioceReportController report = new InvioceReportController(_iPrintService, _ireportFileService);

        [HttpGet("GetFilesForPrint")]
        public async Task<ReportFileRequest> GetFilesForPrint(int screenId, int invoiceId, bool IsArabic)
        {
            var result = await FilemangerService.GetReportPrintFiles(screenId, IsArabic);
            return result;
        }

        [HttpGet("GetAllPrintFile")]
        public async Task<ResponseResult> GetAllPrintFile(int screenId)
        {
            var result = await FilemangerService.GetAllPrintFiles(screenId);
           // report.InvoiceReport(invoiceId,result.Data.)
            return result;
        }
        [HttpGet("SetFileAsDefualt")]
        public async Task<ResponseResult> SetFileAsDefualt(int screenId,int FileId)
        {
            var result = await FilemangerService.SetFileAsDefault(FileId,screenId);
            // report.InvoiceReport(invoiceId,result.Data.)
            return result;
        }
        [HttpGet("GetFileById")]
        public async Task<ResponseResult> GetFileById(int FileId)
        {
            var result = await FilemangerService.GetPrintFileById( FileId);
            return result;
        }
        [HttpGet("GetPOS_RPOS_PrintFilesByDate")]
        public async Task<ResponseResult> GetPOS_RPOS_PrintFilesByDate(DateTime date)
        {
            var result = await FilemangerService.GetPOS_RPOS_PrintFilesByDate(date);
            return result;
        }
        [HttpGet("SetFilesArabicName")]
        public async Task<ResponseResult> SetFilesArabicName()
        {
            var result = await FilemangerService.SetFilesArabicNames();
            return result;
        }
    }
}
