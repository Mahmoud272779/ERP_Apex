using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.UserManagementDB
{
    public partial class ERP_UsersManagerContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ERP_UsersManagerContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ERP_UsersManagerContext(DbContextOptions<ERP_UsersManagerContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AdditionalPrice> AdditionalPrices { get; set; }
        public virtual DbSet<AdditionalPriceSubscription> AdditionalPriceSubscriptions { get; set; }
        public virtual DbSet<AdministrationPage> AdministrationPages { get; set; }
        public virtual DbSet<AdministrationRoleDetail> AdministrationRoleDetails { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<App> Apps { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<BroadCastCompany> BroadCastCompanies { get; set; }
        public virtual DbSet<BroadCastMaster> BroadCastMasters { get; set; }
        public virtual DbSet<Bundle> Bundles { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<ERplog> ERplogs { get; set; }
        public virtual DbSet<EmailSetting> EmailSettings { get; set; }
        public virtual DbSet<EmailsMessage> EmailsMessages { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<GeneralSetting> GeneralSettings { get; set; }
        public virtual DbSet<GenralSetting> GenralSettings { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<OffPriceHistory> OffPriceHistories { get; set; }
        public virtual DbSet<OfferPrice> OfferPrices { get; set; }
        public virtual DbSet<OfferPriceAdditionalItem> OfferPriceAdditionalItems { get; set; }
        public virtual DbSet<OfferPriceApp> OfferPriceApps { get; set; }
        public virtual DbSet<OnlinePaymentTransaction> OnlinePaymentTransactions { get; set; }
        public virtual DbSet<Posversion> Posversions { get; set; }
        public virtual DbSet<RecHistory> RecHistories { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<RulesDetail> RulesDetails { get; set; }
        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<SigninLog> SigninLogs { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<SubReqPeriod> SubReqPeriods { get; set; }
        public virtual DbSet<UserApplication> UserApplications { get; set; }
        public virtual DbSet<UserApplicationApp> UserApplicationApps { get; set; }
        public virtual DbSet<UserApplicationCash> UserApplicationCashes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:UserManagerConnection"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.EmployeesId, "IX_Accounts_EmployeesId");

                entity.HasIndex(e => e.RuleId, "IX_Accounts_RuleId");

                entity.HasIndex(e => e.UserName, "IX_Accounts_UserName")
                    .IsUnique()
                    .HasFilter("([UserName] IS NOT NULL)");

                entity.Property(e => e.AccreditAppsAccounts)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.AccreditAppsTechnicals)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.AllowTrialperiod)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.ArabicName).HasMaxLength(500);

                entity.Property(e => e.BindTrialAccounts)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.CancelOfferPrice)
                    .IsRequired()
                    .HasColumnName("cancelOfferPrice")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.ConfirmApps)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.SendOfferPriceToSalesman)
                    .IsRequired()
                    .HasColumnName("sendOfferPriceToSalesman")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendOfferPriceToSubRequest)
                    .IsRequired()
                    .HasColumnName("sendOfferPriceToSubRequest")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.UserName).HasMaxLength(200);

                entity.HasOne(d => d.Employees)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.EmployeesId);

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RuleId);
            });

            modelBuilder.Entity<AdditionalPrice>(entity =>
            {
                entity.ToTable("additionalPrice");

                entity.HasIndex(e => e.ArabicName, "IX_additionalPrice_ArabicName")
                    .IsUnique();

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.ArabicName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.LatinName).HasMaxLength(500);

                entity.Property(e => e.MonthPrice).HasDefaultValueSql("((0.000000000000000e+000))");

                entity.Property(e => e.SpecialApps).HasColumnName("specialApps");

                entity.HasMany(d => d.Apps)
                    .WithMany(p => p.AdditionalPrices)
                    .UsingEntity<Dictionary<string, object>>(
                        "AdditionalPriceApp",
                        l => l.HasOne<App>().WithMany().HasForeignKey("AppId"),
                        r => r.HasOne<AdditionalPrice>().WithMany().HasForeignKey("AdditionalPriceId"),
                        j =>
                        {
                            j.HasKey("AdditionalPriceId", "AppId");

                            j.ToTable("AdditionalPriceApps");

                            j.HasIndex(new[] { "AppId" }, "IX_AdditionalPriceApps_AppId");
                        });
            });

            modelBuilder.Entity<AdditionalPriceSubscription>(entity =>
            {
                entity.HasKey(e => new { e.AdditionalPriceId, e.SubRequestId });

                entity.ToTable("additionalPriceSubscription");

                entity.HasIndex(e => e.SubRequestId, "IX_additionalPriceSubscription_subRequestID");

                entity.Property(e => e.SubRequestId).HasColumnName("subRequestID");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.HasOne(d => d.AdditionalPrice)
                    .WithMany(p => p.AdditionalPriceSubscriptions)
                    .HasForeignKey(d => d.AdditionalPriceId);

                entity.HasOne(d => d.SubRequest)
                    .WithMany(p => p.AdditionalPriceSubscriptions)
                    .HasForeignKey(d => d.SubRequestId);
            });

            modelBuilder.Entity<AdministrationPage>(entity =>
            {
                entity.HasKey(e => e.PageId);

                entity.Property(e => e.Url).HasColumnName("URL");
            });

            modelBuilder.Entity<AdministrationRoleDetail>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PageId });

                entity.HasIndex(e => e.PageId, "IX_AdministrationRoleDetails_PageId");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.AdministrationRoleDetails)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AdministrationRoleDetails)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<App>(entity =>
            {
                entity.Property(e => e.ArabicName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.LatinName).HasMaxLength(500);

                entity.Property(e => e.MonthlyPrice)
                    .HasColumnName("monthlyPrice")
                    .HasDefaultValueSql("((0.000000000000000e+000))");

                entity.Property(e => e.YealryPrice).HasColumnName("yealryPrice");

                entity.HasMany(d => d.AppChildren)
                    .WithMany(p => p.AppParents)
                    .UsingEntity<Dictionary<string, object>>(
                        "Appsrelation",
                        l => l.HasOne<App>().WithMany().HasForeignKey("AppChildId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<App>().WithMany().HasForeignKey("AppParentId").OnDelete(DeleteBehavior.ClientSetNull),
                        j =>
                        {
                            j.HasKey("AppChildId", "AppParentId");

                            j.ToTable("Appsrelation");

                            j.HasIndex(new[] { "AppParentId" }, "IX_Appsrelation_appParentId");

                            j.IndexerProperty<int>("AppChildId").HasColumnName("appChildId");

                            j.IndexerProperty<int>("AppParentId").HasColumnName("appParentId");
                        });

                entity.HasMany(d => d.AppParents)
                    .WithMany(p => p.AppChildren)
                    .UsingEntity<Dictionary<string, object>>(
                        "Appsrelation",
                        l => l.HasOne<App>().WithMany().HasForeignKey("AppParentId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<App>().WithMany().HasForeignKey("AppChildId").OnDelete(DeleteBehavior.ClientSetNull),
                        j =>
                        {
                            j.HasKey("AppChildId", "AppParentId");

                            j.ToTable("Appsrelation");

                            j.HasIndex(new[] { "AppParentId" }, "IX_Appsrelation_appParentId");

                            j.IndexerProperty<int>("AppChildId").HasColumnName("appChildId");

                            j.IndexerProperty<int>("AppParentId").HasColumnName("appParentId");
                        });
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("applicationRoles");
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("applicationUsers");

                entity.HasIndex(e => e.CompanyId, "IX_applicationUsers_CompanyId");

                entity.HasIndex(e => e.RoleId, "IX_applicationUsers_RoleId");

                entity.Property(e => e.IsDefaultUser)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ApplicationUsers)
                    .HasForeignKey(d => d.CompanyId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.ApplicationUsers)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch");
            });

            modelBuilder.Entity<BroadCastCompany>(entity =>
            {
                entity.HasIndex(e => e.BroadCastMasterId, "IX_BroadCastCompanies_BroadCastMasterId");

                entity.HasIndex(e => e.CompanyId, "IX_BroadCastCompanies_companyId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.HasOne(d => d.BroadCastMaster)
                    .WithMany(p => p.BroadCastCompanies)
                    .HasForeignKey(d => d.BroadCastMasterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.BroadCastCompanies)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BroadCastMaster>(entity =>
            {
                entity.ToTable("BroadCastMaster");

                entity.HasIndex(e => e.UserId, "IX_BroadCastMaster_userId");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.CDate).HasColumnName("cDate");

                entity.Property(e => e.ForAll)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BroadCastMasters)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Bundle>(entity =>
            {
                entity.ToTable("bundles");

                entity.Property(e => e.AllowedNumberOfPos).HasColumnName("AllowedNumberOfPOS");

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.IsInfinityItems)
                    .IsRequired()
                    .HasColumnName("isInfinityItems")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.IsInfinityNumbersOfApps).HasColumnName("isInfinityNumbersOfApps");

                entity.Property(e => e.IsInfinityNumbersOfBranchs).HasColumnName("isInfinityNumbersOfBranchs");

                entity.Property(e => e.IsInfinityNumbersOfCustomers).HasColumnName("isInfinityNumbersOfCustomers");

                entity.Property(e => e.IsInfinityNumbersOfEmployees).HasColumnName("isInfinityNumbersOfEmployees");

                entity.Property(e => e.IsInfinityNumbersOfInvoices).HasColumnName("isInfinityNumbersOfInvoices");

                entity.Property(e => e.IsInfinityNumbersOfPos).HasColumnName("isInfinityNumbersOfPOS");

                entity.Property(e => e.IsInfinityNumbersOfStores).HasColumnName("isInfinityNumbersOfStores");

                entity.Property(e => e.IsInfinityNumbersOfSuppliers).HasColumnName("isInfinityNumbersOfSuppliers");

                entity.Property(e => e.IsInfinityNumbersOfUsers).HasColumnName("isInfinityNumbersOfUsers");

                entity.Property(e => e.IsPosallowed).HasColumnName("IsPOSAllowed");

                entity.Property(e => e.MonthPrice).HasDefaultValueSql("((0.000000000000000e+000))");

                entity.Property(e => e.Poscount).HasColumnName("POSCount");

                entity.Property(e => e.YearPrice).HasDefaultValueSql("((0.000000000000000e+000))");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("companies");

                entity.HasIndex(e => e.BundleId, "IX_companies_BundleId");

                entity.Property(e => e.EndDate).IsRequired();

                entity.Property(e => e.StartDate).IsRequired();

                entity.Property(e => e.UniqueName).IsRequired();

                entity.HasOne(d => d.Bundle)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.BundleId);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key, "CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ERplog>(entity =>
            {
                entity.ToTable("eRPLogs");

                entity.Property(e => e.ApiPath).HasColumnName("apiPath");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Token).HasColumnName("token");
            });

            modelBuilder.Entity<EmailSetting>(entity =>
            {
                entity.ToTable("emailSettings");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<EmailsMessage>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActivationBody).HasColumnName("activationBody");

                entity.Property(e => e.ActivationSubject).HasColumnName("activationSubject");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.EmployeesId, "IX_Employees_EmployeesId");

                entity.Property(e => e.Email).HasColumnName("EMail");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.SupervisorId).HasColumnName("supervisorId");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Employees)
                    .WithMany(p => p.InverseEmployees)
                    .HasForeignKey(d => d.EmployeesId);
            });

            modelBuilder.Entity<GeneralSetting>(entity =>
            {
                entity.ToTable("GeneralSetting");
            });

            modelBuilder.Entity<GenralSetting>(entity =>
            {
                entity.ToTable("genralSettings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountantEmail).HasColumnName("accountantEmail");

                entity.Property(e => e.GenralManagerEmail).HasColumnName("genralManagerEmail");

                entity.Property(e => e.NotificateClientForActiveSubscraptionInDays)
                    .HasColumnName("notificateClientForActiveSubscraptionInDays")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.OfferPriceSendEmailToAccountantForActiveSubscraption)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.OfferPriceSendEmailToSalesManagerForActiveSubscraption)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.OfferPriceSendEmailToTechncalSupportForActiveSubscraption)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.OfferPriceSendSmstoGenralManagerForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("OfferPriceSendSMSToGenralManagerForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SalesManagerEmail).HasColumnName("salesManagerEmail");

                entity.Property(e => e.SendEmailForSalesManAfterActiveSubscrape)
                    .IsRequired()
                    .HasColumnName("sendEmailForSalesManAfterActiveSubscrape")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendEmailToAccountantForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendEmailToAccountantForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendEmailToClientForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendEmailToClientForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendEmailToSalesManagerForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendEmailToSalesManagerForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendEmailToTechncalSupportForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendEmailToTechncalSupportForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendSmsforSalesManAfterActiveSubscrape)
                    .IsRequired()
                    .HasColumnName("sendSMSForSalesManAfterActiveSubscrape")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendSmstoClientForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendSMSToClientForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SendSmstoGenralManagerForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("sendSMSToGenralManagerForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailAccountantEmail).HasColumnName("trailAccountantEmail");

                entity.Property(e => e.TrailGenralManagerEmail).HasColumnName("trailGenralManagerEmail");

                entity.Property(e => e.TrailNotificateClientForActiveSubscraptionInDays).HasColumnName("trailNotificateClientForActiveSubscraptionInDays");

                entity.Property(e => e.TrailSalesManagerEmail).HasColumnName("trailSalesManagerEmail");

                entity.Property(e => e.TrailSendEmailForSalesManAfterActiveSubscrape)
                    .IsRequired()
                    .HasColumnName("trailSendEmailForSalesManAfterActiveSubscrape")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendEmailToAccountantForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendEmailToAccountantForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendEmailToClientForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendEmailToClientForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendEmailToSalesManagerForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendEmailToSalesManagerForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendEmailToTechncalSupportForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendEmailToTechncalSupportForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendSmsforSalesManAfterActiveSubscrape)
                    .IsRequired()
                    .HasColumnName("trailSendSMSForSalesManAfterActiveSubscrape")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendSmstoClientForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendSMSToClientForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailSendSmstoGenralManagerForActiveSubscraption)
                    .IsRequired()
                    .HasColumnName("trailSendSMSToGenralManagerForActiveSubscraption")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.TrailTechncalSupportEmail).HasColumnName("trailTechncalSupportEmail");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.ArabicName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LatinName).HasMaxLength(200);

                entity.HasMany(d => d.Users)
                    .WithMany(p => p.Modules)
                    .UsingEntity<Dictionary<string, object>>(
                        "InstalledModule",
                        l => l.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
                        r => r.HasOne<Module>().WithMany().HasForeignKey("ModuleId"),
                        j =>
                        {
                            j.HasKey("ModuleId", "UserId");

                            j.ToTable("installedModules");

                            j.HasIndex(new[] { "UserId" }, "IX_installedModules_UserId");
                        });
            });

            modelBuilder.Entity<OffPriceHistory>(entity =>
            {
                entity.ToTable("offPriceHistory");

                entity.HasIndex(e => e.OfferPriceId, "IX_offPriceHistory_offerPriceId");

                entity.HasIndex(e => e.UserId, "IX_offPriceHistory_userId");

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.OfferPriceId).HasColumnName("offerPriceId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.OfferPrice)
                    .WithMany(p => p.OffPriceHistories)
                    .HasForeignKey(d => d.OfferPriceId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OffPriceHistories)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<OfferPrice>(entity =>
            {
                entity.ToTable("offerPrice");

                entity.HasIndex(e => e.BundleId, "IX_offerPrice_bundleId");

                entity.Property(e => e.BundleId).HasColumnName("bundleId");

                entity.Property(e => e.CDate)
                    .HasColumnName("cDate")
                    .HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.CancelReason).HasColumnName("cancelReason");

                entity.Property(e => e.City).HasColumnName("city");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.CompanyActive).HasColumnName("companyActive");

                entity.Property(e => e.CompanyName).HasColumnName("companyName");

                entity.Property(e => e.Net).HasColumnName("net");

                entity.Property(e => e.Period).HasColumnName("period");

                entity.Property(e => e.PeriodType).HasColumnName("periodType");

                entity.Property(e => e.PersonName).HasColumnName("personName");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.SalesmanId).HasColumnName("salesmanId");

                entity.Property(e => e.Statues).HasColumnName("statues");

                entity.Property(e => e.Total).HasColumnName("total");

                entity.Property(e => e.Vat).HasColumnName("vat");

                entity.HasOne(d => d.Bundle)
                    .WithMany(p => p.OfferPrices)
                    .HasForeignKey(d => d.BundleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OfferPriceAdditionalItem>(entity =>
            {
                entity.ToTable("offerPrice_AdditionalItems");

                entity.HasIndex(e => e.AdditionalItemId, "IX_offerPrice_AdditionalItems_additionalItemId");

                entity.HasIndex(e => e.OfferPriceId, "IX_offerPrice_AdditionalItems_offerPriceId");

                entity.Property(e => e.AdditionalItemId).HasColumnName("additionalItemId");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.OfferPriceId).HasColumnName("offerPriceId");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.HasOne(d => d.AdditionalItem)
                    .WithMany(p => p.OfferPriceAdditionalItems)
                    .HasForeignKey(d => d.AdditionalItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OfferPrice)
                    .WithMany(p => p.OfferPriceAdditionalItems)
                    .HasForeignKey(d => d.OfferPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OfferPriceApp>(entity =>
            {
                entity.ToTable("offerPrice_Apps");

                entity.HasIndex(e => e.AppId, "IX_offerPrice_Apps_AppId");

                entity.HasIndex(e => e.OfferPriceId, "IX_offerPrice_Apps_offerPriceId");

                entity.Property(e => e.OfferPriceId).HasColumnName("offerPriceId");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.OfferPriceApps)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OfferPrice)
                    .WithMany(p => p.OfferPriceApps)
                    .HasForeignKey(d => d.OfferPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OnlinePaymentTransaction>(entity =>
            {
                entity.ToTable("onlinePaymentTransactions");

                entity.HasIndex(e => e.UserApplicationCashId, "IX_onlinePaymentTransactions_userApplication_CashID");

                entity.Property(e => e.CashTransactionId).HasColumnName("cashTransactionId");

                entity.Property(e => e.InvoiceStatues).HasColumnName("invoiceStatues");

                entity.Property(e => e.Last4DigitsOfVisaCard).HasColumnName("last4DigitsOfVisaCard");

                entity.Property(e => e.OnlinePaymentId).HasColumnName("onlinePaymentId");

                entity.Property(e => e.PaymentInvoiceId).HasColumnName("paymentInvoiceId");

                entity.Property(e => e.PaymentUrl).HasColumnName("PaymentURL");

                entity.Property(e => e.TotalWithFormat).HasColumnName("totalWithFormat");

                entity.Property(e => e.UserApplicationCashId).HasColumnName("userApplication_CashID");

                entity.Property(e => e.VisaCardOwnerName).HasColumnName("visaCardOwnerName");

                entity.HasOne(d => d.UserApplicationCash)
                    .WithMany(p => p.OnlinePaymentTransactions)
                    .HasForeignKey(d => d.UserApplicationCashId);
            });

            modelBuilder.Entity<Posversion>(entity =>
            {
                entity.ToTable("POSVersions");
            });

            modelBuilder.Entity<RecHistory>(entity =>
            {
                entity.ToTable("RecHistory");

                entity.Property(e => e.Os).HasColumnName("OS");
            });

            modelBuilder.Entity<Rule>(entity =>
            {
                entity.Property(e => e.ArabicName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(1)))");

                entity.Property(e => e.LatinName).HasMaxLength(500);
            });

            modelBuilder.Entity<RulesDetail>(entity =>
            {
                entity.HasKey(e => new { e.RuleId, e.FormName });

                entity.Property(e => e.CanAdd)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.CanDelete)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.CanEdit)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.CanOpen)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.CanPrint)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.RulesDetails)
                    .HasForeignKey(d => d.RuleId);
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SigninLog>(entity =>
            {
                entity.ToTable("signinLogs");

                entity.HasIndex(e => e.UserId, "IX_signinLogs_userID");

                entity.Property(e => e.IsLogout)
                    .IsRequired()
                    .HasColumnName("isLogout")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.LogoutTime).HasColumnName("logoutTime");

                entity.Property(e => e.SigninTime)
                    .HasColumnName("signinTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SigninLogs)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<SubReqPeriod>(entity =>
            {
                entity.ToTable("subReqPeriod");

                entity.HasIndex(e => e.CompanyId, "IX_subReqPeriod_CompanyID");

                entity.HasIndex(e => e.SubReqId, "IX_subReqPeriod_SubReqID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Seen)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.SubReqId).HasColumnName("SubReqID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SubReqPeriods)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubReq)
                    .WithMany(p => p.SubReqPeriods)
                    .HasForeignKey(d => d.SubReqId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserApplication>(entity =>
            {
                entity.ToTable("UserApplication");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CDate).HasColumnName("cDate");

                entity.Property(e => e.CityName).HasColumnName("cityName");

                entity.Property(e => e.CompanyActivity).HasColumnName("companyActivity");

                entity.Property(e => e.CompanyLogin).HasColumnName("companyLogin");

                entity.Property(e => e.CompanyNameEn).HasColumnName("companyName_En");

                entity.Property(e => e.Country).HasColumnName("country");

                entity.Property(e => e.DatabaseName).HasColumnName("databaseName");

                entity.Property(e => e.EmployeesNumber).HasColumnName("employeesNumber");

                entity.Property(e => e.IsDatabaseCreating)
                    .IsRequired()
                    .HasColumnName("isDatabaseCreating")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Username).HasColumnName("username");

                entity.Property(e => e.Vatno).HasColumnName("VATNO");
            });

            modelBuilder.Entity<UserApplicationApp>(entity =>
            {
                entity.ToTable("UserApplication_Apps");

                entity.HasIndex(e => e.AppId, "IX_UserApplication_Apps_AppID");

                entity.HasIndex(e => e.ReqId, "IX_UserApplication_Apps_ReqID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.ReqId).HasColumnName("ReqID");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.UserApplicationApps)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Req)
                    .WithMany(p => p.UserApplicationApps)
                    .HasForeignKey(d => d.ReqId);
            });

            modelBuilder.Entity<UserApplicationCash>(entity =>
            {
                entity.ToTable("userApplication_Cash");

                entity.HasIndex(e => e.AccAccountId, "IX_userApplication_Cash_Acc_AccountID");

                entity.HasIndex(e => e.AccManagerId, "IX_userApplication_Cash_Acc_Manager_ID");

                entity.HasIndex(e => e.AccTechId, "IX_userApplication_Cash_Acc_Tech_ID");

                entity.HasIndex(e => e.BundlesId, "IX_userApplication_Cash_BundlesID");

                entity.HasIndex(e => e.UserApplicationId, "IX_userApplication_Cash_userApplicationID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccAccountDateTime).HasColumnName("Acc_Account_DateTime");

                entity.Property(e => e.AccAccountId).HasColumnName("Acc_AccountID");

                entity.Property(e => e.AccManagerDateTime).HasColumnName("Acc_Manager_DateTime");

                entity.Property(e => e.AccManagerId).HasColumnName("Acc_Manager_ID");

                entity.Property(e => e.AccTechDateTime).HasColumnName("Acc_Tech_DateTime");

                entity.Property(e => e.AccTechId).HasColumnName("Acc_Tech_ID");

                entity.Property(e => e.AccountantApproved).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountantRefuseNote).HasColumnName("accountantRefuseNote");

                entity.Property(e => e.AllowedNumberOfEmployeesOfBundle).HasColumnName("allowedNumberOfEmployeesOfBundle");

                entity.Property(e => e.AllowedNumberOfPosofBundle).HasColumnName("allowedNumberOfPOSOfBundle");

                entity.Property(e => e.AllowedNumberOfStoresOfBundle).HasColumnName("allowedNumberOfStoresOfBundle");

                entity.Property(e => e.AllowedNumberOfUsersOfBundle).HasColumnName("allowedNumberOfUsersOfBundle");

                entity.Property(e => e.BillingReceive).HasColumnName("billingReceive");

                entity.Property(e => e.BundlesId).HasColumnName("BundlesID");

                entity.Property(e => e.Cdate)
                    .HasColumnName("cdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImageUrl).HasColumnName("imageURL");

                entity.Property(e => e.IsInfinityItems)
                    .IsRequired()
                    .HasColumnName("isInfinityItems")
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.IsInfinityNumbersOfApps).HasColumnName("isInfinityNumbersOfApps");

                entity.Property(e => e.IsInfinityNumbersOfBranchs).HasColumnName("isInfinityNumbersOfBranchs");

                entity.Property(e => e.IsInfinityNumbersOfCustomers).HasColumnName("isInfinityNumbersOfCustomers");

                entity.Property(e => e.IsInfinityNumbersOfEmployees).HasColumnName("isInfinityNumbersOfEmployees");

                entity.Property(e => e.IsInfinityNumbersOfInvoices).HasColumnName("isInfinityNumbersOfInvoices");

                entity.Property(e => e.IsInfinityNumbersOfPos).HasColumnName("isInfinityNumbersOfPOS");

                entity.Property(e => e.IsInfinityNumbersOfStores).HasColumnName("isInfinityNumbersOfStores");

                entity.Property(e => e.IsInfinityNumbersOfSuppliers).HasColumnName("isInfinityNumbersOfSuppliers");

                entity.Property(e => e.IsInfinityNumbersOfUsers).HasColumnName("isInfinityNumbersOfUsers");

                entity.Property(e => e.IsTrail).HasColumnName("isTrail");

                entity.Property(e => e.ManagerConfirmation).HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Months).HasColumnName("months");

                entity.Property(e => e.OnlinePaymentSignalRconnectionId).HasColumnName("onlinePaymentSignalRConnectionId");

                entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");

                entity.Property(e => e.PaymentNumber).HasColumnName("paymentNumber");

                entity.Property(e => e.PeriodType).HasColumnName("periodType");

                entity.Property(e => e.RecNumber).HasColumnName("recNumber");

                entity.Property(e => e.SubReqType).HasColumnName("subReqType");

                entity.Property(e => e.SubTypeAr).HasColumnName("subTypeAr");

                entity.Property(e => e.SubTypeEn).HasColumnName("subTypeEN");

                entity.Property(e => e.TechnicalSupportApproved).HasDefaultValueSql("((0))");

                entity.Property(e => e.Total).HasColumnName("total");

                entity.Property(e => e.TrasferVia).HasColumnName("trasferVia");

                entity.Property(e => e.UpgradeType).HasColumnName("upgradeType");

                entity.Property(e => e.UserApplicationId).HasColumnName("userApplicationID");

                entity.Property(e => e.Vat).HasColumnName("vat");

                entity.HasOne(d => d.AccAccount)
                    .WithMany(p => p.UserApplicationCashAccAccounts)
                    .HasForeignKey(d => d.AccAccountId);

                entity.HasOne(d => d.AccManager)
                    .WithMany(p => p.UserApplicationCashAccManagers)
                    .HasForeignKey(d => d.AccManagerId);

                entity.HasOne(d => d.AccTech)
                    .WithMany(p => p.UserApplicationCashAccTeches)
                    .HasForeignKey(d => d.AccTechId);

                entity.HasOne(d => d.Bundles)
                    .WithMany(p => p.UserApplicationCashes)
                    .HasForeignKey(d => d.BundlesId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserApplication)
                    .WithMany(p => p.UserApplicationCashes)
                    .HasForeignKey(d => d.UserApplicationId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
