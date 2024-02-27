using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    internal class updateNum6
    {
    
        public static async void Update_6(ClientSqlDbContext dbContext,IConfiguration _configuration, IWebHostEnvironment webHostEnvironment)
        {


            await Method1_AddBranchsPersonFund(dbContext, _configuration);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 6;


            dbContext.SaveChanges();
        }
        private async static Task Method1_AddBranchsPersonFund(ClientSqlDbContext dbContext, IConfiguration _configuration)
        {
            var branches = dbContext.branchs.AsTracking().Where(c => c.Id != 1).ToList();
            var connectionString = ConnectionString.connectionString(_configuration, dbContext.Connection.Database);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                foreach (var item in branches)
                {
                    var AddSQLQuery = $"INSERT INTO [dbo].[InvFundsCustomerSupplier]([PersonId],[Credit],[Debit],[branchId]) select Id,0,0,{item.Id} from InvPersons where not exists(select Id from [InvFundsCustomerSupplier] where branchId = {item.Id})";
                    con.Execute(AddSQLQuery);
                    con.Execute("update [GLJournalEntry] set BranchId = 1 where Id = -2 or Id = -3");
                    con.Execute("update [GLJournalEntry] set DocType = 32 where Id = -2;");
                    con.Execute("update [GLJournalEntry] set DocType = 33 where Id = -3");

            //await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 6);
                }
                dbContext.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }
        }
        //khaled
        
    }
}
