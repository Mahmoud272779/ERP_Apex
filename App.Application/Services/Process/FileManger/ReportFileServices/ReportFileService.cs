using App.Domain;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.FileManger.ReportFileServices
{
    public class ReportFileService : BaseClass , IReportFileService
    {
        private readonly IRepositoryCommand<ReportFiles> _reportCommand;
        private readonly  IRepositoryQuery<ReportFiles> reportQuery;

        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        public ReportFileService(IRepositoryCommand<ReportFiles> reportCommand,
            IHttpContextAccessor httpContext, IMapper mapper,
            IRepositoryQuery<ReportFiles> reportQuery) : base(httpContext)
        {
            _reportCommand = reportCommand;
            _httpContext = httpContext;
            _mapper = mapper;
            this.reportQuery = reportQuery;
        }

        public async Task<IEnumerable<ReportFiles>> GetAllReportFiles()
        {
            var reportFiles = await reportQuery.GetAllAsyn();
            return reportFiles;
        }

        public async Task<ResponseResult> SaveReort(ReportFileRequest ReportFileRequest)
        {
            var report = _mapper.Map<ReportFiles>(ReportFileRequest);

            await _reportCommand.AddAsync(report);
            await _reportCommand.SaveAsync();
            //await _reportCommand.();


            return new ResponseResult() { Data = null, Id = report.Id, Result = Result.Success };
        }

        public async Task<ResponseResult> UpdateReport(ReportFileRequest reportFileRequest)
        {
            if (reportFileRequest.Id == 0)
            {
               
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            }
            var reportData = await reportQuery.GetByAsync(r => r.Id == reportFileRequest.Id &&  r.IsArabic == reportFileRequest.IsArabic);
            if (reportData == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            }
            
            reportData.IsReport = reportFileRequest.IsReport;
            reportData.IsArabic = reportFileRequest.IsArabic;
            reportFileRequest.ReportFileName = reportFileRequest.ReportFileName;
            reportData.Files = reportFileRequest.Files;
            await _reportCommand.UpdateAsyn(reportData);

            return new ResponseResult() { Data = null, Id = reportData.Id, Result = reportData == null ? Result.Failed : Result.Success };

        }
    }
}
