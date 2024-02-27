using App.Infrastructure.Persistence.Context;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    public class updateNum3
    {
        public async static Task Update_3(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {

            method_1_FixAccredit(dbContext);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 3;
            dbContext.SaveChanges();
        }

        private async static Task method_1_FixAccredit(ClientSqlDbContext dbContext)
        {
            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
            try
            {
                con.Open();
                var query = "update [GlReciepts]  set IsAccredit =  (select IsAccredite from InvoiceMaster i where i.InvoiceId = [GlReciepts].ParentId and [GlReciepts].ParentId is not null) where GlReciepts.ParentId is not null and GlReciepts.ParentId !=0;";
                query += "delete from [InvoiceMasterHistory] where LastAction = 'ACC' and InvoiceType = (select InvoiceType from InvoiceMaster i where i.IsAccredite = 0 and i.InvoiceId = [InvoiceMasterHistory].InvoiceId);";
                con.Execute(query);
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
    }
}
