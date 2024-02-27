using App.Domain;
using App.Domain.Common;
using App.Domain.Entities;
using App.Domain.Entities.Notification;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Process.Store.Barcode;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Context;
using App.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Threading;
using static App.Domain.Entities.Chat.chat;
using System.Data.SqlClient;

namespace App.Infrastructure.Persistence.Context
{
    public class ClientSqlDbContext : DbContext, IApplicationSqlDbContext
    {
       
        //private readonly string connectionString;
        private static IHttpContextAccessor httpContextAccessor;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IErpInitilizerData _initilizerData;
        OnModelCreatingService _onModelCreatingService;

        #region DBSet
        public DbSet<GLBranch> branchs { get; set; }
        public DbSet<GLBank> banks { get; set; }
        public DbSet<GLFinancialAccount> financialAccounts { get; set; }
        public DbSet<GLSafe> safes { get; set; }
        public DbSet<InvColors> colors { get; set; }
        public DbSet<InvSizes> sizes { get; set; }
        public DbSet<InvStpUnits> units { get; set; }
        public DbSet<InvJobs> jobs { get; set; }
        public DbSet<InvStorePlaces> storePlaces { get; set; }
        public DbSet<InvStpStores> stores { get; set; }
        public DbSet<InvCategories> categories { get; set; }
        public DbSet<InvEmployees> employees { get; set; }
        public DbSet<InvEmployeeBranch> employeeBranch { get; set; }
        public DbSet<InvGeneralSettings> invGeneralSettings { get; set; }
        public DbSet<InvSalesMan> salesman { get; set; }
        public DbSet<InvSalesMan_Branches> SalesManBranches { get; set; }

        public DbSet<InvPersons> person { get; set; }
        public DbSet<InvPersons_Branches> PersonsBranches { get; set; }
        public DbSet<InvPaymentMethods> paymentMethod { get; set; }
        public DbSet<InvFundsCustomerSupplier> invFundsCustomerSuppliers { get; set; }

        public DbSet<InvStpItemCardMaster> itemCards { get; set; }
        public DbSet<InvStpItemCardUnit> InvStpItemCardUnits { get; set; }

        public DbSet<InvCompanyData> companyData { get; set; }
        public DbSet<InvBarcodeTemplate> invBarcodeTemplates { get; set; }
        public DbSet<InvBarcodeItems> invBarcodeItems { get; set; }
        public DbSet<GLOtherAuthorities> OtherAuthorities { get; set; }
        public DbSet<GLOtherAuthoritiesHistory> OtherAuthoritiesHistory { get; set; }
        public DbSet<GLGeneralSetting> gLGeneralSettings { get; set; }
        public DbSet<GLJournalEntry> gLJournalEntries { get; set; }
        public DbSet<InvStoreBranch> StoreBranches { get; set; }
        public DbSet<EditedItems> EditedItems { get; set; }
        public DbSet<userAccount> userAccount { get; set; }
        public DbSet<permissionList> permissionLists { get; set; }
        public DbSet<rules> rules { get; set; }
        public DbSet<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettings { get; set; }
        public DbSet<POSDevices> POSDevices { get; set; }
        public DbSet<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }
        public DbSet<POSInvSuspensionDetails> POSInvSuspensionDetails { get; set; }
        public DbSet<SystemHistoryLogs> systemHistoryLogs { get; set; }
        public DbSet<ReportFiles> reportFiles { get; set; }
        public DbSet<ScreenName> screenNames { get; set; }
        public DbSet<ReportManger> reportMangers { get; set; }
        public DbSet<signinLogs> signinLogs { get; set; }
        public DbSet<signalR> signalR { get; set; }
        public DbSet<POSSession> pOSSessions { get; set; }
        public DbSet<chatMessages> chatMessages { get; set; }
        public DbSet<chatGroups> chatGroups { get; set; }
        public DbSet<chatGroupMembers> chatGroupMembers { get; set; }
        public DbSet<usersForgetPassword> usersForgetPasswords { get; set; }
        public DbSet<InvoiceDetails> invoiceDetails { get; set; }
        public DbSet<OtherSettingsStores> otherSettingsStores { get; set; }
        public DbSet<POSTouchSettings> POSTouchSettings { get; set; }
        public DbSet<POSSessionHistory> pOSSessionHistories { get; set; }
        public DbSet<DeletedRecords> DeletedRecords { get; set; }
        public DbSet<BarcodePrintFiles> BarcodePrintFiles { get; set; }
        public DbSet<NotificationsMaster> NotificationsMaster { get; set; }
        public DbSet<NotificationSeen> NotificationSeen { get; set; }
        public DbSet<POS_OfflineDevices> POS_OfflineDevices { get; set; }
        public DbSet<GLPrinter> GLPrinter { get; set; }
        public DbSet<GLBankBranch> gLBankBranches { get; set; }
        public DbSet<GLCostCenter> gLCostCenters { get; set; }
        public DbSet<GLCurrency> gLCurrencies { get; set; }
        public DbSet<GLFinancialBranch> gLFinancialBranches { get; set; }
        public DbSet<SubCodeLevels> subCodeLevels { get; set; }
        public DbSet<GLIntegrationSettings> gLIntegrationSettings { get; set; }
        public DbSet<UserAndPermission> userAndPermissions { get; set; }
        public DbSet<otherSettings> otherSettings { get; set; }
        public DbSet<OtherSettingsBanks> otherSettingsBanks { get; set; }
        public DbSet<OtherSettingsSafes> otherSettingsSafes { get; set; }
        public DbSet<Kitchens> Kitchens { get; set; }
        public DbSet<KitchensHistory> KitchensHistory { get; set; }
        public DbSet<GLPrinterHistory> GLPrinterHistory { get; set; }
        public DbSet<InvoiceMaster> invoiceMasters { get; set; }
        public DbSet<GlReciepts> GlReciepts { get; set; }






        #endregion


        public ClientSqlDbContext(DbContextOptions<ClientSqlDbContext> options,
            IHttpContextAccessor _httpContextAccessor,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IErpInitilizerData erpInitilizerData) : base(options)
        {
            httpContextAccessor = _httpContextAccessor;
            _configuration = configuration;
            _initilizerData = erpInitilizerData;
            _onModelCreatingService = new OnModelCreatingService(_initilizerData);
        }

    
        public IDbConnection Connection => Database.GetDbConnection();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                if (optionsBuilder.IsConfigured)
                {
                   
                    if (httpContextAccessor.HttpContext != null)
                    {
                        var token = httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
                        if (token != null)
                        {
                            var dbName = StringEncryption.DecryptString(contextHelper.GetDBName(token));
                            var connString = 
                                            @"Data Source=" + _configuration["ApplicationSetting:serverName"] + ";" +
                                            "Initial Catalog=" + dbName + ";" +
                                            "user id=" + _configuration["ApplicationSetting:UID"] + ";" +
                                            "password=" + _configuration["ApplicationSetting:Password"] + ";" +
                                            "MultipleActiveResultSets=true;";
                            optionsBuilder.UseSqlServer(connString);
                        }
                        else
                        {
                            var allowedAPI = allowedAPIs.allowedAPIS();
                            var endPoint = httpContextAccessor.HttpContext.Request.Path.ToString().Split('/').Last().ToLower();
                            if (!allowedAPI.Where(x=> x.APIName.ToLower() == endPoint).Any())
                                errorResponse.responseUnautorized(httpContextAccessor.HttpContext);
                        }
                    }
                }
        }
        public void ClearConnectionPool()
        {
            var connectionString = Database.GetConnectionString();
            SqlConnection con = new SqlConnection(connectionString);
            // Clear the connection pool
            SqlConnection.ClearPool(con);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IApplicationSqlDbContext).Assembly);

            //if (defultData.isMigration)
            //    base.OnModelCreating(modelBuilder);
            #region Seed Data
            //_onModelCreatingService.OnModelCreating(modelBuilder);
            #endregion

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is AuditableEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));
            BaseClass BaseClass = new BaseClass(httpContextAccessor);


            //#region Commented By MuhammadSalem OutSide
            //if (entries.Any() && httpContextAccessor.HttpContext != null)
            //{

            //    //string userName = BaseClass.ApplicationUser.UserName;
            //    //int Branch = BaseClass.ApplicationUser.BRANCH_ID;
            //    #region Commented By MuhammadSalem inside
            //    foreach (EntityEntry entityEntry in entries)
            //    {
            //        ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ", new CultureInfo("en-us"));
            //        //  ((AuditableEntity)entityEntry.Entity).LAST_TRANSACTION_USER = userName;
            //        ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

            //        if (entityEntry.State == EntityState.Added)
            //        {
            //            ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
            //            //((AuditableEntity)entityEntry.Entity).ADD_TRANSACTION_USER = userName;
            //            ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
            //            //((AuditableEntity)entityEntry.Entity).BRANCH_ID = Branch;

            //        }
            //    }
            //    #endregion

            //}
            //#endregion

            return base.SaveChangesAsync();
        }
        public override int SaveChanges()
        {
            IEnumerable<EntityEntry> entries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is AuditableEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));
            BaseClass BaseClass = new BaseClass(httpContextAccessor);



            //if (entries.Any() && httpContextAccessor.HttpContext != null)
            //{

            //    // string userName = BaseClass.ApplicationUser.UserName;
            //    // int Branch = BaseClass.ApplicationUser.BRANCH_ID;
            //    foreach (EntityEntry entityEntry in entries)
            //    {
            //        if (((AuditableEntity)entityEntry.Entity).LastTransactionDate != null)
            //            continue;

            //        ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
            //        //  ((AuditableEntity)entityEntry.Entity).LAST_TRANSACTION_USER = userName;
            //       // ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

            //        if (entityEntry.State == EntityState.Added)
            //        {
            //            ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
            //            // ((AuditableEntity)entityEntry.Entity).ADD_TRANSACTION_USER = userName;
            //           // ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
            //            //((AuditableEntity)entityEntry.Entity).BRANCH_ID = Branch;

            //        }
            //    }
            //}

            return base.SaveChanges();
        }
    }
}
