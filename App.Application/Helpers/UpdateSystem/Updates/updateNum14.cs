using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum14
    {
        public static async void Update_14(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await Method_1_UpdatePrintFiles(dbContext, webHostEnvironment);
            
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 14;

            dbContext.SaveChanges();
        }

        private async static Task Method_1_UpdatePrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var filesToUpdate = GetListOfFile.ReportFilesList().Where(a => a.screenId == (int)SubFormsIds.DebtAgingForCustomers
            || a.screenId == (int)SubFormsIds.DebtAgingForSupplier).FirstOrDefault();

            var fileNamesToUpdate = dbContext.reportMangers.Include(r => r.Files).Where(r => (r.screenId == (int)SubFormsIds.DebtAgingForCustomers ||
            r.screenId == (int)SubFormsIds.DebtAgingForSupplier)
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
