using App.Infrastructure.Persistence.Context;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Data.SqlClient;


namespace App.Application.Helpers.UpdateSystem.Updates
{
    public class updateNum2
    {
        public async static Task Update_2(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {

            method_1_updateCostinInvoiceDetailsForEachItem(dbContext,webHostEnvironment);
            method_2_updateRulesSetPrintersAsResturantApplication(dbContext, webHostEnvironment);
            method_3_fixRulesForEntryFund(dbContext, webHostEnvironment);
            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 2;
            dbContext.SaveChanges();
        }

        private async static Task method_1_updateCostinInvoiceDetailsForEachItem(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var branches = dbContext.branchs;
            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
            con.Open();
            con.Execute("delete EditedItems");
            var query = string.Empty;

            query = $"INSERT INTO EditedItems(itemId,sizeId,type,serialize,BranchID) " +
                    $"select  distinct " +
                    $"d.ItemId, " +
                    $"'0' as 'sideId', " +
                    $"d.ItemTypeId, " +
                    $"'1' as 'serialize'," +
                    $"m.BranchId  " +
                    $"from InvoiceDetails d join InvoiceMaster m on m.InvoiceId = d.InvoiceId where d.ItemTypeId != 5 and d.ItemTypeId != 6 and d.ItemTypeId != 0 group by d.ItemId,d.SizeId,m.BranchId,d.ItemTypeId;";
            con.Execute(query);
            con.Close();
        }

        private async static Task method_2_updateRulesSetPrintersAsResturantApplication(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            var printerRule = dbContext.rules.AsNoTracking().Where(x => x.subFormCode == (int)SubFormsIds.Printers && x.applicationId == (int)applicationIds.Genral).ToList();
            if (!printerRule.Any())
                return;
            printerRule.ForEach(c => c.applicationId = (int)applicationIds.Restaurant);
            dbContext.rules.UpdateRange(printerRule);
            dbContext.SaveChanges();
        }
        private async static Task method_3_fixRulesForEntryFund(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
            con.Open();
            con.Execute("update [rules] set isShow = 1 where permissionListId = 1 and mainFormCode = 1 ");
            con.Close();
        }
    }
}
