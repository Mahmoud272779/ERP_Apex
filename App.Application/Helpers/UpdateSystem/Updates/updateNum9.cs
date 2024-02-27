using App.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum9
    {
        public static async void Update_9(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await setFilesAsDefault(dbContext, webHostEnvironment);
            //await Update_PaymentMethods_Withdrawal_from_the_beneficiarys_balance(dbContext);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 9;



            dbContext.SaveChanges();
        }

        private async static Task setFilesAsDefault(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var fileNamesFromDb = dbContext.reportFiles.Where(a=>a.ReportFileName== "DetailedInvoices"||
            a.ReportFileName == "TransferReceiptForSafe" ||a.ReportFileName == "TransferReceiptForBank"
            ).ToList();
            foreach (var file in fileNamesFromDb)
            {
                file.IsDefault = true;
            }
            dbContext.reportFiles.UpdateRange(fileNamesFromDb);


        }
        //private async static Task Update_PaymentMethods_Withdrawal_from_the_beneficiarys_balance(ClientSqlDbContext dbContext)
        //{
        //    var paymentMethod = dbContext.paymentMethod.FirstOrDefault(c => c.PaymentMethodId == -1);
        //    paymentMethod.LatinName = "Withdrawal from the beneficiarys balance";
        //    dbContext.paymentMethod.Update(paymentMethod);
        //    dbContext.SaveChanges();
        //}
    }
}
