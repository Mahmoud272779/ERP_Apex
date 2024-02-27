using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class UserApplicationCash
    {
        public UserApplicationCash()
        {
            AdditionalPriceSubscriptions = new HashSet<AdditionalPriceSubscription>();
            OnlinePaymentTransactions = new HashSet<OnlinePaymentTransaction>();
            SubReqPeriods = new HashSet<SubReqPeriod>();
            UserApplicationApps = new HashSet<UserApplicationApp>();
        }

        public int Id { get; set; }
        public int UserApplicationId { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime Cdate { get; set; }
        public string Notes { get; set; }
        public string TrasferVia { get; set; }
        public bool? BillingReceive { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTrail { get; set; }
        public int BundlesId { get; set; }
        public double BundlePrice { get; set; }
        public int? AccAccountId { get; set; }
        public DateTime AccAccountDateTime { get; set; }
        public int? AccountantApproved { get; set; }
        public string AccountantRefuseNote { get; set; }
        public int? AccTechId { get; set; }
        public DateTime AccTechDateTime { get; set; }
        public int? TechnicalSupportApproved { get; set; }
        public string TechnicalSupportRefuseNote { get; set; }
        public int? AccManagerId { get; set; }
        public DateTime AccManagerDateTime { get; set; }
        public bool? ManagerConfirmation { get; set; }
        public bool? TrailApproved { get; set; }
        public DateTime? TrailApprovedDateTime { get; set; }
        public int PaymentMethod { get; set; }
        public int Months { get; set; }
        public double Total { get; set; }
        public double Vat { get; set; }
        public double Net { get; set; }
        public string RecNumber { get; set; }
        public int SubReqType { get; set; }
        public string SubTypeAr { get; set; }
        public string SubTypeEn { get; set; }
        public bool IsInfinityNumbersOfEmployees { get; set; }
        public int AllowedNumberOfEmployeesOfBundle { get; set; }
        public bool IsInfinityNumbersOfApps { get; set; }
        public int AllowedNumberOfApps { get; set; }
        public bool IsInfinityNumbersOfStores { get; set; }
        public int AllowedNumberOfStoresOfBundle { get; set; }
        public bool IsInfinityNumbersOfUsers { get; set; }
        public int AllowedNumberOfUsersOfBundle { get; set; }
        public bool IsInfinityNumbersOfPos { get; set; }
        public int AllowedNumberOfPosofBundle { get; set; }
        public bool IsInfinityNumbersOfBranchs { get; set; }
        public int AllowedNumberOfBranchs { get; set; }
        public bool IsInfinityNumbersOfInvoices { get; set; }
        public int AllowedNumberOfInvoices { get; set; }
        public bool IsInfinityNumbersOfSuppliers { get; set; }
        public int AllowedNumberOfSuppliers { get; set; }
        public bool IsInfinityNumbersOfCustomers { get; set; }
        public int AllowedNumberOfCustomers { get; set; }
        public int? UpgradeType { get; set; }
        public int AllowedNumberOfItems { get; set; }
        public bool? IsInfinityItems { get; set; }
        public string OnlinePaymentSignalRconnectionId { get; set; }
        public int PeriodType { get; set; }

        public virtual Account AccAccount { get; set; }
        public virtual Account AccManager { get; set; }
        public virtual Account AccTech { get; set; }
        public virtual Bundle Bundles { get; set; }
        public virtual UserApplication UserApplication { get; set; }
        public virtual ICollection<AdditionalPriceSubscription> AdditionalPriceSubscriptions { get; set; }
        public virtual ICollection<OnlinePaymentTransaction> OnlinePaymentTransactions { get; set; }
        public virtual ICollection<SubReqPeriod> SubReqPeriods { get; set; }
        public virtual ICollection<UserApplicationApp> UserApplicationApps { get; set; }
    }
}
