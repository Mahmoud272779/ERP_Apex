using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class Update_forTaifSoft
    {
    
        public static async void UpdateforTaifSoft(ClientSqlDbContext dbContext , string dbName)
        {
            if(dbName == "Apex_taifsoft2024_2023_2_20231113163935")
            {
                dbContext.invGeneralSettings.FirstOrDefault().isExpenses = true;
                dbContext.SaveChanges();
            }
        }

    

    }
}
