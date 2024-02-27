using App.Domain.Models.Response.General;
using App.Domain.Models.Request;
using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.PrintFile
{
    public interface IprintFileService
    {
        Task<ReportsReponse> PrintFile(WebReport webReport, string SavedFileName, exportType _exportType, bool isPOS = false, bool barcode = false);
    }
}
