using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum7
    {
    
        public static async void Update_7(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
             await method_2_AddScreenNamesAndPrintFiles(dbContext, webHostEnvironment);

            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 7;
            dbContext.invGeneralSettings.FirstOrDefault().Sales_ModifyPricesType = 2;
            dbContext.invGeneralSettings.FirstOrDefault().Pos_ModifyPricesType = 2;


            dbContext.SaveChanges();
        }

        private async static Task method_2_AddScreenNamesAndPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {

            await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 7);
        }
        //khaled

    }
}
