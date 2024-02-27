using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace App.Application.Helpers.UpdateSystem.Updates
{
    public class SetSystemUpdateNumber
    {

        public static async void SetUpdateNumber(ClientSqlDbContext dbContext,int updateNumber )
        {
            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());

            try
            {

                con.Open();



                var query = $"update  InvGeneralSettings set SystemUpdateNumber  ='{updateNumber}';";
                con.Execute(query);
            }
            catch (Exception e)
            {

            }
            finally
            {
                con.Close();

            }
        }
    }
}
