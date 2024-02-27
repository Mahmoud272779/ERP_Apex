using App.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum10
    {
        public static async void Update_10(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await Method_1_UpdateprintFiles(dbContext, webHostEnvironment);

            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 10;



            dbContext.SaveChanges();
        }

        private async static Task Method_1_UpdateprintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var filesToUpdate = GetListOfFile.ReportFilesList().Where(a => a.screenId == (int)SubFormsIds.TotalSalesOfBranch).FirstOrDefault();

            var fileNamesToUpdate = dbContext.reportMangers.Include(r => r.Files).Where(r => r.screenId == (int)SubFormsIds.TotalSalesOfBranch
            && r.Files.ReportFileName == filesToUpdate.reportName).Select(r => r.Files).ToList();
            foreach (var file in fileNamesToUpdate)
            {
                if (file.IsArabic == true)
                {
                    file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, true);
                }
                else
                {
                    file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, false);
                }
            }
            dbContext.reportFiles.UpdateRange(fileNamesToUpdate);

        }
    }
}
