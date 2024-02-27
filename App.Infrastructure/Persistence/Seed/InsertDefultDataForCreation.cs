using App.Domain;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace App.Infrastructure.Persistence.Seed
{
    public class InsertDefultDataForCreation 
    {
        private readonly IErpInitilizerData _iErpInitilizerData;
        public InsertDefultDataForCreation(IErpInitilizerData iErpInitilizerData)
        {
            _iErpInitilizerData = iErpInitilizerData;
        }

        public async Task InsertDefultData(ClientSqlDbContext _dbContext)
        {

            InsertBranchsData(_dbContext);

            InsertCurrencyData(_dbContext);

            InsertFinancialAccountData(_dbContext);

            InsertStoresData(_dbContext);

            InsertBankData(_dbContext);

            InsertSafesData(_dbContext);

            InsertGLJournalEntryData(_dbContext);

            InsertEmployeesData(_dbContext);

            InsertUsersAndPermissionData(_dbContext);

            InsertItemCardTables(_dbContext);

            InsertCostCenterData(_dbContext);

            InsertPersonData(_dbContext);

            InsertPaymentMethodData(_dbContext);

            InsertSettingsData(_dbContext);
            InsertKitchens(_dbContext);

            //InsertPaymentMethod_PersonBalance(_dbContext);





            //last method set updatenumber remember dont put any method after this
            SetUpdateNumber(_dbContext);
        }

     

        private async void addPrinters(ClientSqlDbContext _dbContext)
        {
            //code
        }
        private async void InsertBranchsData(ClientSqlDbContext _dbContext )
        {
            await PrepareInsertIdentityInsert<GLBranch>.Prepare(_iErpInitilizerData.ReturnBranchTypeList(), _dbContext, "GLBranch");
        }
        private async void InsertCurrencyData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLCurrency>.Prepare(_iErpInitilizerData.ReturnWithCurrencyList(), _dbContext, "GLCurrency");
        }
        private async void InsertFinancialAccountData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLFinancialAccount>.Prepare(_iErpInitilizerData.ReturnWithFinancialAccountList(), _dbContext, "GLFinancialAccount");


            var data = _iErpInitilizerData.ReturnFinancialAccountBranches();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [GLFinancialBranch] ([BranchId], [FinancialId], [FinancialAccountId]) VALUES ({item.BranchId}, {item.FinancialId}, NULL);";
            }
            con.Execute(query);
            con.Close();
        }
        private async void InsertStoresData(ClientSqlDbContext _dbContext)
        {
           await PrepareInsertIdentityInsert<InvStpStores>.Prepare(_iErpInitilizerData.setDefultStores(), _dbContext, nameof(InvStpStores));
           await PrepareInsertIdentityInsert<InvStorePlaces>.Prepare(_iErpInitilizerData.setDefultStoresPlaces(), _dbContext,nameof(InvStorePlaces));

            var data = _iErpInitilizerData.setStoreBranch();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [InvStoreBranch] ([BranchId], [StoreId]) VALUES ({item.BranchId}, {item.StoreId});";
            }
            con.Execute(query);
            con.Close();
        }
        private async void InsertBankData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLBank>.Prepare(_iErpInitilizerData.ReturnBanksTypeList(), _dbContext, "GLBanks");

            var data = _iErpInitilizerData.GLBankBranch();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [GLBankBranch] ([BankId], [BranchId]) VALUES ({item.BankId}, {item.BranchId});";
            }
            con.Execute(query);
            con.Close();

        }
        private async void InsertSafesData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLSafe>.Prepare(_iErpInitilizerData.ReturnTreasuryTypeList(), _dbContext, "GLSafe");

        }
        private async void InsertGLJournalEntryData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLJournalEntry>.Prepare(_iErpInitilizerData.SetGLJournalEntry(), _dbContext,"GLJournalEntry");
        }
        private async void InsertEmployeesData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<InvJobs>.Prepare(_iErpInitilizerData.setInvJobs(), _dbContext, "InvJobs");
            await PrepareInsertIdentityInsert<InvEmployees>.Prepare(_iErpInitilizerData.setDefultEmployees(), _dbContext, "InvEmployees");


            var data = _iErpInitilizerData.setEmployeesBranches();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [InvEmployeesBranches] ([BranchId], [EmployeeId], [current]) VALUES ({item.BranchId}, {item.EmployeeId}, CAST(0 AS bit));";
            }
            con.Execute(query);
            con.Close();
        }
        private async void InsertUsersAndPermissionData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<permissionList>.Prepare(_iErpInitilizerData.setPermissionLists(), _dbContext, "permissionList");
            await PrepareInsertIdentityInsert<userAccount>.Prepare(_iErpInitilizerData.SetUserAccount(), _dbContext, "userAccount");
            await PrepareInsertIdentityInsert<UserAndPermission>.Prepare(_iErpInitilizerData.setUserAndPermissions(), _dbContext, "UserAndPermission");
            await PrepareInsertIdentityInsert<otherSettings>.Prepare(_iErpInitilizerData.setOtherSettings(), _dbContext, "otherSettings");
            await PrepareInsertIdentityInsert<OtherSettingsBanks>.Prepare(_iErpInitilizerData.setOtherSettingBanks(), _dbContext, "OtherSettingsBanks");
            await PrepareInsertIdentityInsert<OtherSettingsSafes>.Prepare(_iErpInitilizerData.setOtherSettingSafes(), _dbContext, "OtherSettingsSafes");
            await PrepareInsertIdentityInsert<OtherSettingsStores>.Prepare(_iErpInitilizerData.setOtherSettingStores(), _dbContext, "OtherSettingsStores");
            await PrepareInsertIdentityInsert<rules>.Prepare(_iErpInitilizerData.setRules(), _dbContext, "rules");
        }
        private async void InsertItemCardTables(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<InvSizes>.Prepare(_iErpInitilizerData.setInvSizes(), _dbContext, "InvSizes");
            await PrepareInsertIdentityInsert<InvStpUnits>.Prepare(_iErpInitilizerData.setInvStpUnits(), _dbContext, "InvStpUnits");
            await PrepareInsertIdentityInsert<InvCategories>.Prepare(_iErpInitilizerData.setInvCategories(), _dbContext, "InvCategories");
            await PrepareInsertIdentityInsert<InvStpItemCardMaster>.Prepare(_iErpInitilizerData.setInvStpItemCardMaster(), _dbContext, "InvStpItemMaster");

            var data = _iErpInitilizerData.setInvStpItemCardUnit();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [InvStpItemUnit] ([ItemId], [UnitId], [Barcode], [ConversionFactor], [PurchasePrice], [SalePrice1], [SalePrice2], [SalePrice3], [SalePrice4], [WillDelete]) " +
                    $"VALUES ({item.ItemId}, {item.UnitId}, N'', {item.ConversionFactor}, {item.PurchasePrice}, {item.SalePrice1}, {item.SalePrice2}, {item.SalePrice3}, {item.SalePrice4}, CAST(0 AS bit));";
            }
            con.Execute(query);
            con.Close();




            await PrepareInsertIdentityInsert<InvColors>.Prepare(_iErpInitilizerData.setColors(), _dbContext, "InvColors");
        }
        private async void InsertCostCenterData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLCostCenter>.Prepare(_iErpInitilizerData.setGLCostCenter(), _dbContext, "GLCostCenter");
        }
        private async void InsertPersonData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<InvSalesMan>.Prepare(_iErpInitilizerData.setInvSalesMan(), _dbContext, "InvSalesMan");

            var data = _iErpInitilizerData.setInvSalesMan_Branches();
            SqlConnection con = new SqlConnection(_dbContext.Database.GetConnectionString());
            con.Open();
            string query = string.Empty;
            foreach (var item in data)
            {
                query += $"INSERT INTO [InvSalesMan_Branches] ([BranchId], [SalesManId]) VALUES ({item.BranchId}, {item.SalesManId});";
            }
            con.Execute(query);
            con.Close();




            await PrepareInsertIdentityInsert<InvPersons>.Prepare(_iErpInitilizerData.SetInvPersons(), _dbContext, "InvPersons");



            var data_InvPersons_Branches = _iErpInitilizerData.setInvPersons_Branches();
            string query_InvPersons_Branches = string.Empty;
            foreach (var item in data_InvPersons_Branches)
            {
                query_InvPersons_Branches += $"INSERT INTO [InvPersons_Branches] ([BranchId], [PersonId]) VALUES ({item.BranchId}, {item.PersonId});";
            }
            con.Execute(query_InvPersons_Branches);







            await PrepareInsertIdentityInsert<InvFundsCustomerSupplier>.Prepare(_iErpInitilizerData.setInvFundsCustomerSupplier(), _dbContext, "InvFundsCustomerSupplier");            
            con.Close();
        }
        private async void InsertPaymentMethodData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<InvPaymentMethods>.Prepare(_iErpInitilizerData.setInvPaymentMethods(), _dbContext, "InvPaymentMethods");
        }
        private async void SetUpdateNumber(ClientSqlDbContext _dbContext)
        {
            //Insert update system number 
            var settings = _dbContext.invGeneralSettings.AsNoTracking().FirstOrDefault();
            settings.SystemUpdateNumber = defultData.updateNumber;
            _dbContext.invGeneralSettings.Update(settings);
            _dbContext.SaveChanges();
        }
        private async void InsertSettingsData(ClientSqlDbContext _dbContext)
        {
            await PrepareInsertIdentityInsert<GLGeneralSetting>.Prepare(_iErpInitilizerData.SetGlGeneralSettings(), _dbContext, "GLGeneralSetting");
            await PrepareInsertIdentityInsert<SubCodeLevels>.Prepare(_iErpInitilizerData.setSubCodeLevels(), _dbContext, "SubCodeLevels");
            await PrepareInsertIdentityInsert<InvCompanyData>.Prepare(_iErpInitilizerData.setInvCompanyData(), _dbContext, "InvCompanyData");
            await PrepareInsertIdentityInsert<InvGeneralSettings>.Prepare(_iErpInitilizerData.setInvGeneralSettings(), _dbContext, "InvGeneralSettings");
            await PrepareInsertIdentityInsert<GLPurchasesAndSalesSettings>.Prepare(_iErpInitilizerData.setGLPurchasesAndSalesSettings(), _dbContext, "GLPurchasesAndSalesSettings");
            await PrepareInsertIdentityInsert<GLOtherAuthorities>.Prepare(_iErpInitilizerData.setGLOtherAuthorities(), _dbContext, "OtherAuthorities");
            await PrepareInsertIdentityInsert<ScreenName>.Prepare(_iErpInitilizerData.setScreenName(), _dbContext, "screenNames");
            await PrepareInsertIdentityInsert<GLIntegrationSettings>.Prepare(_iErpInitilizerData.setGLIntegrationSettings(), _dbContext, "GLIntegrationSettings");
        }
        public async void InsertKitchens(ClientSqlDbContext _dbContext)
        {

        }
    }
    public class PrepareInsertIdentityInsert<T> where T : class
    {
        public async static Task Prepare( T[] data, ClientSqlDbContext _dbContext,string tableName)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                var aa = dbContextTransaction.TransactionId;
                try
                {
                    try
                    {
                        _dbContext.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON;");
                    }
                    catch (Exception)
                    {

                    }


                    _dbContext.Set<T>().AddRange(data);
                    _dbContext.SaveChanges();


                    try
                    {
                        _dbContext.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF;");
                    }
                    catch (Exception)
                    {

                    }
                    
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    throw ex;

                }
            }
        }
    }
}
