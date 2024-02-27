using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum13
    {
        public static async void Update_13(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await Method_1_AddPrintFilesAndScreenNames(dbContext, webHostEnvironment);
            
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 13;



            dbContext.SaveChanges();
        }

        private async static Task Method_1_AddPrintFilesAndScreenNames(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var screenIds = dbContext.screenNames.Select(a => a.Id).ToList();
            var screens = await UpdateScreenNames.UpdateScreens(screenIds);
            await PrepareInsertIdentityInsert<ScreenName>.Prepare(screens.ToArray(), dbContext, "screenNames");
            await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 13);
        }

        
    }
}
