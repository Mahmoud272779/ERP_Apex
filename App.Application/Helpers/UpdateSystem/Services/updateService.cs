using App.Application.Helpers.UpdateSystem.Updates;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace App.Application.Helpers.UpdateSystem.Services
{
    public class updateService : iUpdateService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IErpInitilizerData erpInitializerData;
        private readonly IConfiguration _configuration;
        public updateService(IHttpContextAccessor httpContext, IWebHostEnvironment webHostEnvironment, IErpInitilizerData erpInitializerData, IConfiguration configuration)
        {
            _httpContext = httpContext;
            _webHostEnvironment = webHostEnvironment;
            this.erpInitializerData = erpInitializerData;
            _configuration = configuration;
        }



        public async Task UpdateDatabase(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment , string dbName)
        {
            var DatabaseUpdateNumber = dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber;
            if(DatabaseUpdateNumber < defultData.updateNumber)
            {
                if(DatabaseUpdateNumber < 1)
                {
                    updateNum1.Update_1(dbContext, webHostEnvironment);
                    
                }
                if(DatabaseUpdateNumber < 2)
                {
                    updateNum2.Update_2(dbContext, webHostEnvironment);
                    
                }
                if (DatabaseUpdateNumber < 3)
                {
                    updateNum3.Update_3(dbContext, webHostEnvironment);
                    
                }
                if (DatabaseUpdateNumber < 4)
                {
                    updateNum4.Update_4(dbContext, erpInitializerData, webHostEnvironment);
                    
                }
                if (DatabaseUpdateNumber < 5)
                {
                    updateNum5.Update_5(dbContext);

                }
                if (DatabaseUpdateNumber < 6)
                {
                    updateNum6.Update_6(dbContext, _configuration , webHostEnvironment);

                }

                if (DatabaseUpdateNumber < 7)
                {
                    updateNum7.Update_7(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 8)
                {
                    updateNum8.Update_8(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 9)
                {
                    updateNum9.Update_9(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 10)
                {
                    updateNum10.Update_10(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 11)
                {
                    updateNum11.Update_11(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 12)
                {
                    updateNum12.Update_12(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 13)
                {
                    updateNum13.Update_13(dbContext, webHostEnvironment);

                }
                if (DatabaseUpdateNumber < 14)
                {
                    updateNum14.Update_14(dbContext, webHostEnvironment);

                }
                //Update_forTaifSoft.UpdateforTaifSoft(dbContext, dbName);

            }

        }

        public async Task updateFile(ClientSqlDbContext dbContext, int updateFilesNumber)
        {
            //update number 1
           // await updateNum1.Update_1(dbContext);
            //update Report Files
            await ReportFilesUpdate.AddPrintFiles(dbContext, _webHostEnvironment);
            //if (updateFilesNumber < 1)
            //{
            //    await ReportFilesUpdate.AddBarcodeFiles(dbContext, _webHostEnvironment);

            //}

            await ReportFilesUpdate.UpdatePrintFiles(dbContext, _webHostEnvironment);
            //await updateNum1.Update_2(dbContext);
        }
    }
}
