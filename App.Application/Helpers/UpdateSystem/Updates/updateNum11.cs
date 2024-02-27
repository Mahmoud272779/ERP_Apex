using App.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum11
    {
        public static async void Update_11(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await Method_1_UpdateprintFiles(dbContext, webHostEnvironment);
            await Method_2_Update_PaymentMethods_Withdrawal_from_the_beneficiarys_balance(dbContext);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 11;



            dbContext.SaveChanges();
        }

        private async static Task Method_1_UpdateprintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {

            await ReportFilesUpdate.UpdateSpecificFile(dbContext, _webHostEnvironment, (int)SubFormsIds.PaymentsAndDisbursements);
            //var filesToUpdate = GetListOfFile.ReportFilesList().Where(a => a.screenId == (int)SubFormsIds.PaymentsAndDisbursements).FirstOrDefault();

            //var fileNamesToUpdate = dbContext.reportMangers.Include(r => r.Files).Where(r => r.screenId == (int)SubFormsIds.PaymentsAndDisbursements
            //&& r.Files.ReportFileName == filesToUpdate.reportName).Select(r => r.Files).ToList();
            //foreach (var file in fileNamesToUpdate)
            //{
            //    if (file.IsArabic == true)
            //    {
            //        file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, true);
            //    }
            //    else
            //    {
            //        file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, false);
            //    }
            //}
            //dbContext.reportFiles.UpdateRange(fileNamesToUpdate);

        }

        private async static Task Method_2_Update_PaymentMethods_Withdrawal_from_the_beneficiarys_balance(ClientSqlDbContext dbContext)
        {
            var paymentMethod = dbContext.paymentMethod.FirstOrDefault(c => c.PaymentMethodId == -1);
            paymentMethod.LatinName = "Withdrawal from the beneficiarys balance";
            dbContext.paymentMethod.Update(paymentMethod);
            dbContext.SaveChanges();
        }
    }
}
