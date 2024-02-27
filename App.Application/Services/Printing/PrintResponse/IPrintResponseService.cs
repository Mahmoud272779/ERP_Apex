using App.Domain.Models.Request.ReportFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.PrintResponse
{
    public interface IPrintResponseService
    {
        Task<ReportsReponse> Print(int invoiceId, int screenId, string invoiceCode, exportType exportType, string employeeNameAr, string employeeNameEn, bool isPOS = false, bool isPriceOffer = false, bool isArabic = true,bool isReport=false);
        Task<ReportFileRequest> PrintBarcode(int screenId);
       


    }
}
