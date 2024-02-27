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
    internal class updateNum5
    {
    
        public static async void Update_5(ClientSqlDbContext dbContext)
        {

            //setPriceListDefaultSetting(dbContext);
            //SetSystemUpdateNumber.SetUpdateNumber(dbContext, 5);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 5;
            dbContext.invGeneralSettings.FirstOrDefault().PriceListType = 1;


            dbContext.SaveChanges();
        }

        //private async static void setPriceListDefaultSetting(ClientSqlDbContext dbContext)
        //{

        //    SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
        //    try
        //    {
        //        con.Open();
        //        var query = "update  InvGeneralSettings set PriceListType  ='1' ;";

        //        con.Execute(query);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }

        //}
        

    }
}
