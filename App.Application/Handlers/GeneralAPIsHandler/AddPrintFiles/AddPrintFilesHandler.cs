using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.AddPrintFiles
{
    public class AddPrintFilesHandler : IRequestHandler<AddPrintFilesRequest, int>
    {
        private readonly IReportFileService _iReportFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepositoryCommand<ReportFiles> _reportFileCommand;
        private readonly IRepositoryCommand<ReportManger> _reportManagerCommand;

        public AddPrintFilesHandler(IReportFileService iReportFileService, IWebHostEnvironment webHostEnvironment, IRepositoryCommand<ReportFiles> reportFileCommand, IRepositoryCommand<ReportManger> reportManagerCommand)
        {
            _iReportFileService = iReportFileService;
            _webHostEnvironment = webHostEnvironment;
            _reportFileCommand = reportFileCommand;
            _reportManagerCommand = reportManagerCommand;
        }

        public async Task<int> Handle(AddPrintFilesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var allFileNames = GetListOfFile.ReportFilesList();
                var fileNamesFromDb = await _iReportFileService.GetAllReportFiles();
                var fileNamesToAdd = new List<ReportFiles>();
                if (allFileNames.Count > 0 && fileNamesFromDb.Count() > 0)
                {
                    foreach (var item in fileNamesFromDb)
                    {

                        foreach (var file in allFileNames)
                        {

                            if (item.ReportFileName == file.reportName)
                            {
                                allFileNames.Remove(file);
                                break;
                            }


                        }
                    }
                }
                if (allFileNames.Count > 0)
                {
                    foreach (var file in allFileNames)
                    {

                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = true,
                            IsReport = 0,
                            ReportFileName = file.reportName,
                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, true)

                        });
                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = false,
                            IsReport = 0,
                            ReportFileName = file.reportName,
                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, false)

                        });

                    }

                    _reportFileCommand.AddRangeAsync(fileNamesToAdd);
                    await _reportFileCommand.SaveChanges();
                    var ReportFileManagerToAdd = new List<ReportManger>();
                    foreach (var item in fileNamesToAdd)
                    {
                        ReportFileManagerToAdd.Add(new ReportManger
                        {
                            IsArabic = item.IsArabic,
                            Copies = 1,
                            ArabicFilenameId = item.Id,
                            screenId = allFileNames.Where(x => x.reportName == item.ReportFileName).FirstOrDefault().screenId
                        });
                    }

                    _reportManagerCommand.AddRangeAsync(ReportFileManagerToAdd);
                    _reportManagerCommand.SaveChanges();

                }
                return Defults.updateNumber;

            }
            catch (Exception)
            {
                return 0;
            }


        }
    }
}
