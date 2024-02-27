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
    internal class updateNum8
    {
    
        public static async void Update_8(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
             await method_1_AddScreenNamesAndPrintFiles(dbContext, webHostEnvironment);

            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 8;
           


            dbContext.SaveChanges();
        }

        private async static Task method_1_AddScreenNamesAndPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {

            await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 8);
        }
        //khaled

    }
}
