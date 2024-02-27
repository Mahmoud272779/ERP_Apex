using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Seed;
using App.Infrastructure.settings;
using Dapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    public static class updateNum1
    {
        public async static Task Update_1(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            await Method1_GLPurchasesAndSalesSettingsAddCreditorFundsItems(dbContext);
            await method_2_UpGeneralLedgerPermissionIntegration(dbContext);
            await method_3_AddScreenNamesAndPrintFiles(dbContext, webHostEnvironment);
            await method_4_SetCollectionReceiptsForSuperAdmin(dbContext);
            //await method_5_addPrintFiles(dbContext, webHostEnvironment);
            await method_5_copyNotesInInvoiceMaster_To_InvoiceNotesInGLReciept(dbContext);
            await method_6_updateCostinInvoiceDetailsForEachItem(dbContext, webHostEnvironment);

            dbContext.invGeneralSettings.AsNoTracking().FirstOrDefault().SystemUpdateNumber = 1;
            dbContext.SaveChanges();
        }
       


        private async static Task Method1_GLPurchasesAndSalesSettingsAddCreditorFundsItems(ClientSqlDbContext dbContext)
        {
            var branches = dbContext.branchs;
            var listOfGLPurchasesAndSalesSettings = defultData.New_getlistOfGLPurchasesAndSalesSettingsTable();
            var GLPurchasesAndSalesSettingsDB = dbContext.GLPurchasesAndSalesSettings;
            var listOfGLPurchasesForAdd = new List<GLPurchasesAndSalesSettings>();

            var itemFundCreditor = listOfGLPurchasesAndSalesSettings.Where(x => x.RecieptsType == (int)DocumentType.itemsFund && x.ReceiptElemntID == (int)DebitoAndCredito.creditor).FirstOrDefault();
            foreach (var item in branches)
            {
                if (item.Id == 1)
                    continue;
                //check if element exist
                if (GLPurchasesAndSalesSettingsDB.Where(x => x.branchId == item.Id && x.RecieptsType == (int)DocumentType.itemsFund && x.ReceiptElemntID == (int)DebitoAndCredito.creditor).Any())
                    continue;


                listOfGLPurchasesForAdd.Add(new GLPurchasesAndSalesSettings()
                {
                    ArabicName = itemFundCreditor.ArabicName,
                    LatinName = itemFundCreditor.LatinName,
                    FinancialAccountId = itemFundCreditor.FinancialAccountId,
                    MainType = itemFundCreditor.MainType,
                    ReceiptElemntID = itemFundCreditor.ReceiptElemntID,
                    branchId = item.Id,
                    RecieptsType = itemFundCreditor.RecieptsType
                });
            }
            if (listOfGLPurchasesForAdd.Any())
            {
                dbContext.GLPurchasesAndSalesSettings.AddRange(listOfGLPurchasesForAdd);
                var saved = await dbContext.SaveChangesAsync();
            }
        }

        private async static Task method_2_UpGeneralLedgerPermissionIntegration(ClientSqlDbContext dbContext)
        {
            var creditElemets = dbContext.GLPurchasesAndSalesSettings.AsNoTracking().Where(c => c.RecieptsType == (int)DocumentType.AddPermission || c.RecieptsType == (int)DocumentType.ExtractPermission).ToList();
            creditElemets.ForEach(c => c.ReceiptElemntID = (int)DebitoAndCredito.creditor);
            dbContext.GLPurchasesAndSalesSettings.UpdateRange(creditElemets);
            dbContext.SaveChanges();



            var branches = dbContext.branchs;
            List<GLPurchasesAndSalesSettings> listOfData = new List<GLPurchasesAndSalesSettings>();
            var data = defultData.New_getlistOfGLPurchasesAndSalesSettingsTable().Where(c => c.Id == -23 || c.Id == -24).ToList();
            var financialAccounts = dbContext.financialAccounts;
            var GLPurchasesAndSalesSettings = dbContext.GLPurchasesAndSalesSettings.AsNoTracking();
            foreach (var item in branches)
            {
                var isExist = financialAccounts.Where(c => c.Id == data.FirstOrDefault().FinancialAccountId).Any();
                foreach (var it in data)
                {
                    it.FinancialAccountId = isExist ? it.FinancialAccountId : -501007;
                    if (!financialAccounts.Any(c => c.Id == it.FinancialAccountId))
                        it.FinancialAccountId = -102006001;
                    if (GLPurchasesAndSalesSettings.Where(c => c.branchId == item.Id && c.ReceiptElemntID == it.ReceiptElemntID && c.RecieptsType == it.RecieptsType).Any())
                        continue;
                    listOfData.Add(new GLPurchasesAndSalesSettings
                    {
                        ArabicName = it.ArabicName,
                        FinancialAccountId = it.FinancialAccountId,
                        LatinName = it.LatinName,
                        MainType = it.MainType,
                        ReceiptElemntID = it.ReceiptElemntID,
                        RecieptsType = it.RecieptsType,
                        branchId = item.Id
                    });
                }
            }
            dbContext.GLPurchasesAndSalesSettings.AddRange(listOfData);
            dbContext.SaveChanges();
        }

        private async static Task method_3_AddScreenNamesAndPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            //int[] subFormsIds = new int[] { 128,129,130, 131 };
            var screenIds=dbContext.screenNames.Select(a=>a.Id).ToList();
            var screens = await UpdateScreenNames.UpdateScreens(screenIds);

            await PrepareInsertIdentityInsert<ScreenName>.Prepare(screens.ToArray(), dbContext, "screenNames");

            await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 1);
        }
        private async static Task method_4_SetCollectionReceiptsForSuperAdmin(ClientSqlDbContext dbContext)
        {
            var superAdminSettings = dbContext.otherSettings.Where(c => c.userAccountId == 1).FirstOrDefault();
            if (superAdminSettings != null)
            {
                superAdminSettings.CollectionReceipts = true;
                dbContext.SaveChanges();
            }
        }

        //private async static Task method_5_addPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        //{
        //    await ReportFilesUpdate.AddPrintFilesForUpdate(dbContext, _webHostEnvironment, 1);
        //}

        private async static Task method_5_copyNotesInInvoiceMaster_To_InvoiceNotesInGLReciept(ClientSqlDbContext dbContext)
        {
            SqlConnection con = new SqlConnection(dbContext.Database.GetConnectionString());
            con.Open();
            con.Execute("update GlReciepts set Notes = (select  Notes from InvoiceMaster where InvoiceMaster.InvoiceId = GlReciepts.ParentId) where ParentTypeId = 8 or ParentTypeId = 11 or ParentTypeId = 9 or ParentTypeId = 12 or ParentTypeId = 5 or ParentTypeId = 6");
            con.Close();



            //var invoicesNotes = dbContext.invoiceMasters.AsNoTracking().Where(c => Lists.invociesTransaction.Contains(c.InvoiceTypeId));
            //var recs = dbContext.GlReciepts.Where(c => Lists.invociesTransaction.Contains(c.ParentTypeId ?? 0) && c.CollectionCode == 0 && c.InvoiceNotes == null);
            //foreach (var item in recs)
            //{
            //    item.InvoiceNotes = invoicesNotes.Where(c => c.InvoiceId == item.ParentId).FirstOrDefault().Notes;
            //}
            //dbContext.GlReciepts.UpdateRange(recs);
            //dbContext.SaveChanges();

            

        }


        private async static Task method_6_updateCostinInvoiceDetailsForEachItem(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
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


        //private async static Task Method_1_Update_2_UpdateRulesTable_MainFormId(ClientSqlDbContext dbContext)
        //{
        //    var listOfRules = returnSubForms.returnRules();
        //    var rules = dbContext.rules.ToList();
        //    rules.ForEach(x =>
        //    {
        //        x.mainFormCode = listOfRules.Where(c => c.subFormCode == x.subFormCode).FirstOrDefault().mainFormCode;
        //    });
        //    dbContext.rules.UpdateRange(rules);
        //    dbContext.SaveChanges();
        //}

    }
}
