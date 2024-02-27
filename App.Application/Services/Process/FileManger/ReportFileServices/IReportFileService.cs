using App.Domain;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.FileManger.ReportFileServices
{
    public interface IReportFileService
    {
        public Task<ResponseResult> SaveReort(ReportFileRequest ReportFileRequest);
        public Task<ResponseResult> UpdateReport(ReportFileRequest ReportFileRequest);
        public Task<IEnumerable<ReportFiles>> GetAllReportFiles();

    }
}
