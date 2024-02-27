using App.Domain.Common;
using App.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App.Infrastructure.Context
{
    public class ApplicationOracleDbContext : DbContext, IApplicationOracleDbContext
    {
        private static IHttpContextAccessor httpContextAccessor;

        public ApplicationOracleDbContext()
        {
        }

        public ApplicationOracleDbContext(DbContextOptions<ApplicationOracleDbContext> options, IHttpContextAccessor _httpContextAccessor)
            : base(options)
        {
            httpContextAccessor = _httpContextAccessor;

        }
        public IDbConnection Connection => Database.GetDbConnection();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationOracleDbContext).Assembly);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker
      .Entries()
      .Where(e => e.Entity is AuditableEntity && (
              e.State == EntityState.Added
              || e.State == EntityState.Modified));
            BaseClass BaseClass = new BaseClass(httpContextAccessor);



            if (entries.Any() && httpContextAccessor.HttpContext != null)
            {

                string userName = BaseClass.ApplicationUser.UserName;
                int Branch = BaseClass.ApplicationUser.BRANCH_ID;
                foreach (EntityEntry entityEntry in entries)
                {
                    ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-us"));
                    ((AuditableEntity)entityEntry.Entity).LastTransactionUser = userName;
                    ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-us"));
                        ((AuditableEntity)entityEntry.Entity).AddTransactionUser = userName;
                        ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
                        ((AuditableEntity)entityEntry.Entity).BranchId = Branch;

                    }
                }
            }


            return base.SaveChangesAsync();
        }
        //public override int SaveChangesAsync()
        //{
        //    return 1;
        //}
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

                string userName = BaseClass.ApplicationUser.UserName;
                int Branch = BaseClass.ApplicationUser.BRANCH_ID;
                foreach (EntityEntry entityEntry in entries)
                {
                    ((AuditableEntity)entityEntry.Entity).LastTransactionDate = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-us"));
                    ((AuditableEntity)entityEntry.Entity).LastTransactionUser = userName;
                    ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "U";

                    if (entityEntry.State == EntityState.Added)
                    {
                        ((AuditableEntity)entityEntry.Entity).AddTransactionDate = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-us"));
                        ((AuditableEntity)entityEntry.Entity).AddTransactionUser = userName;
                        ((AuditableEntity)entityEntry.Entity).LastTransactionAction = "A";
                        ((AuditableEntity)entityEntry.Entity).BranchId = Branch;

                    }
                }
            }


            return base.SaveChanges();
        }
    }
}
