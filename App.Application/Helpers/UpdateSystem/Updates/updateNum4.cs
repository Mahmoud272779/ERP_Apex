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
    internal class updateNum4
    {
    
        public static async void Update_4(ClientSqlDbContext dbContext, IErpInitilizerData _iErpInitilizerData, IWebHostEnvironment webHostEnvironment)
        {
            Mehtod_1_InsertPaymentMethod_PersonBalance(dbContext, _iErpInitilizerData);
            method_2_AddScreenNamesAndPrintFiles(dbContext, webHostEnvironment);
            method_3_setPriceList(dbContext);

            Mehtod_4_AddPaymentMethod(dbContext, _iErpInitilizerData);

            dbContext.invGeneralSettings.FirstOrDefault().SystemUpdateNumber = 4;
            //SetSystemUpdateNumber.SetUpdateNumber(dbContext, 4);
            dbContext.SaveChanges();
        }
        //set methods for update here


        private async static void Mehtod_1_InsertPaymentMethod_PersonBalance(ClientSqlDbContext dbContext, IErpInitilizerData _iErpInitilizerData)
        {
            await PrepareInsertIdentityInsert<InvPaymentMethods>.Prepare(_iErpInitilizerData.setInvPaymentMethods().Where(x=>x.PaymentMethodId== (int)PaymentMethod.PersonBalance).ToArray(), dbContext, "InvPaymentMethods");


        }

        private async static Task method_2_AddScreenNamesAndPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {

            var screenIds = dbContext.screenNames.Select(a => a.Id).ToList();
            var screens = await UpdateScreenNames.UpdateScreens(screenIds);

            await PrepareInsertIdentityInsert<ScreenName>.Prepare(screens.ToArray(), dbContext, "screenNames");

            await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 4);
        }
        private async static void method_3_setPriceList(ClientSqlDbContext dbContext)
        {

            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
            try
            {
                con.Open();
                var query = "update  GLBranch set SalesPriceId  ='1' ;" +
                     "update  InvEmployees set SalesPriceId  ='1' ;" +
                     "update  InvSalesMan set SalesPriceId  ='1' ;" +
                     "update  InvPersons set SalesPriceId  ='1' ;";

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
        private async static void Mehtod_4_AddPaymentMethod(ClientSqlDbContext dbContext, IErpInitilizerData _iErpInitilizerData)
        {

            await PrepareInsertIdentityInsert<InvPaymentMethods>.Prepare(_iErpInitilizerData.setInvPaymentMethods().Where(a => a.PaymentMethodId == (int)PaymentMethod.Transfer).ToArray(), dbContext, "InvPaymentMethods");
        }

    }
}
