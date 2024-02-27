using App.Domain.Models.Request.ReportFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Printing.PrintResponse
{
    public class PrintResponseService : IPrintResponseService
    {
        private readonly IFilesMangerService _filesMangerService;
        public PrintResponseService(IFilesMangerService filesMangerService)
        {
            _filesMangerService = filesMangerService;
        }
        public async Task<ReportsReponse> Print(int invoiceId, int screenId, string invoiceCode, exportType exportType, string employeeNameAr, string employeeNameEn, bool isPOS = false, bool isPriceOffer = false, bool isArabic = true,bool isReport=false)
        {
            InvoiceDTO dto = new InvoiceDTO() { invoiceId = invoiceId, invoiceCode = invoiceCode, screenId = screenId, exportType = exportType, isArabic = isArabic, isPriceOffer = isPriceOffer };
            return await _filesMangerService.GetInviocePrintFile(dto, employeeNameAr, employeeNameEn, isPOS);
        }

        public async Task<ReportFileRequest> PrintBarcode(int screenId)
        {
            return await _filesMangerService.GetBarcodePrintFile(screenId);

        }
    }
}
