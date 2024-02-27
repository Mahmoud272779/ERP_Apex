using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.UpdatePrintFiles
{
    public class UpdatePrintFilesHandler : IRequestHandler<UpdatePrintFilesRequest, int>
    {
        private readonly IReportFileService _iReportFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepositoryCommand<ReportFiles> _reportFileCommand;

        public UpdatePrintFilesHandler(IReportFileService iReportFileService, IWebHostEnvironment webHostEnvironment, IRepositoryCommand<ReportFiles> reportFileCommand)
        {
            _iReportFileService = iReportFileService;
            _webHostEnvironment = webHostEnvironment;
            _reportFileCommand = reportFileCommand;
        }

        public async Task<int> Handle(UpdatePrintFilesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var fileNamesFromDb = await _iReportFileService.GetAllReportFiles();
                var arFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\ar");
                var arfiles = from file in
                Directory.EnumerateFiles(arFilesPath)
                select file;

                var enFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\en");
                var enfiles = from file in
                Directory.EnumerateFiles(enFilesPath)
                              select file;
                string fileName = "";
                foreach (var item in fileNamesFromDb)
                {
                    if (item.IsArabic == true)
                    {
                        foreach (var file in arfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);
                            if (item.ReportFileName == fileName)
                            {
                                // var fileName = Path.GetFileName(file); 
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                break;
                            }
                        }
                    }

                    else
                    {
                        foreach (var file in enfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);

                            if (item.ReportFileName == fileName)
                            {
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                break;
                            }
                        }
                    }
                }
                await _reportFileCommand.UpdateAsyn(fileNamesFromDb);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
