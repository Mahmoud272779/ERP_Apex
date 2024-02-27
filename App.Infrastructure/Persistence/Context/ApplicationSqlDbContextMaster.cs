using App.Domain.Common;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Context;
using App.Infrastructure.Persistence.Seed;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Context
{
    public class ApplicationSqlDbContextMaster : IdentityDbContext, IApplicationSqlDbContextMaster
    {
        //string connectionString = "";
        //public ApplicationSqlDbContextMaster(string _connectionString )
        //{
        //    connectionString = _connectionString;
        //}
        private static IHttpContextAccessor httpContextAccessor;
        private readonly IErpInitilizerData _initilizerData;


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
        public DbSet<InvSerialTransaction> InvSerialTransaction { get; set; }

        public ApplicationSqlDbContextMaster(DbContextOptions<ClientSqlDbContext> options,
            IHttpContextAccessor _httpContextAccessor,
            IErpInitilizerData erpInitilizerData) : base(options)
        {
            httpContextAccessor = _httpContextAccessor;
            _initilizerData = erpInitilizerData;
        }
        public IDbConnection Connection => Database.GetDbConnection();
       // private string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SQLConnectionRunTime").ToString();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder )
        {           
            if (!optionsBuilder.IsConfigured)
            {
            }


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Seed Data
            modelBuilder.Entity<GLBranch>().HasData(_initilizerData.ReturnBranchTypeList());
            modelBuilder.Entity<GLSafe>().HasData(_initilizerData.ReturnTreasuryTypeList());
            modelBuilder.Entity<GLBank>().HasData(_initilizerData.ReturnBanksTypeList());
            modelBuilder.Entity<GLCurrency>().HasData(_initilizerData.ReturnWithCurrencyList());
            modelBuilder.Entity<GLFinancialAccount>().HasData(_initilizerData.ReturnWithFinancialAccountList());
            #endregion
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IApplicationSqlDbContext).Assembly);
            modelBuilder.Entity<GLFinancialBranch>().HasData(_initilizerData.ReturnFinancialAccountBranches());
            modelBuilder.Entity<GLGeneralSetting>().HasData(_initilizerData.SetGlGeneralSettings());

            

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker
      .Entries()
      .Where(e => e.Entity is AuditableEntity && (
              e.State == EntityState.Added
              || e.State == EntityState.Modified));
            BaseClass BaseClass = new BaseClass(httpContextAccessor);


            #region Commented By MuhammadSalem OutSide
            if (entries.Any() && httpContextAccessor.HttpContext != null)
            {

                //string userName = BaseClass.ApplicationUser.UserName;
                //int Branch = BaseClass.ApplicationUser.BRANCH_ID;
                #region Commented By MuhammadSalem inside
                foreach (EntityEntry entityEntry in entries)
                {
                    ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ", new CultureInfo("en-us"));
                    //  ((AuditableEntity)entityEntry.Entity).LAST_TRANSACTION_USER = userName;
                    ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
                        //((AuditableEntity)entityEntry.Entity).ADD_TRANSACTION_USER = userName;
                        ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
                        //((AuditableEntity)entityEntry.Entity).BRANCH_ID = Branch;

                    }
                }
                #endregion

            }
            #endregion

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



            if (entries.Any() && httpContextAccessor.HttpContext != null)
            {

                // string userName = BaseClass.ApplicationUser.UserName;
                // int Branch = BaseClass.ApplicationUser.BRANCH_ID;
                foreach (EntityEntry entityEntry in entries)
                {
                    ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
                    //  ((AuditableEntity)entityEntry.Entity).LAST_TRANSACTION_USER = userName;
                    ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
                        // ((AuditableEntity)entityEntry.Entity).ADD_TRANSACTION_USER = userName;
                        ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
                        //((AuditableEntity)entityEntry.Entity).BRANCH_ID = Branch;

                    }
                }
            }

            return base.SaveChanges();
        }

    }
}
