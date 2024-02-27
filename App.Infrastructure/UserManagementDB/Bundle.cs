using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Bundle
    {
        public Bundle()
        {
            Companies = new HashSet<Company>();
            OfferPrices = new HashSet<OfferPrice>();
            UserApplicationCashes = new HashSet<UserApplicationCash>();
        }

        public int BundleId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool IsInfinityNumbersOfEmployees { get; set; }
        public int AllowedEmployeesNo { get; set; }
        public int TrialDays { get; set; }
        public bool IsInfinityNumbersOfApps { get; set; }
        public int AllowedNumberOfApps { get; set; }
        public bool IsInfinityNumbersOfUsers { get; set; }
        public int AllowedNumberOfUsers { get; set; }
        public bool IsInfinityNumbersOfStores { get; set; }
        public int AllowedNumberOfStores { get; set; }
        public bool IsInfinityNumbersOfBranchs { get; set; }
        public int AllowedNumberOfBranchs { get; set; }
        public bool IsInfinityNumbersOfInvoices { get; set; }
        public int AllowedNumberOfInvoices { get; set; }
        public bool IsInfinityNumbersOfPos { get; set; }
        public int AllowedNumberOfPos { get; set; }
        public bool IsInfinityNumbersOfSuppliers { get; set; }
        public int AllowedNumberOfSuppliers { get; set; }
        public bool IsInfinityNumbersOfCustomers { get; set; }
        public int AllowedNumberOfCustomers { get; set; }
        public bool IsPosallowed { get; set; }
        public int? Poscount { get; set; }
        public int SubscriptionDays { get; set; }
        public bool IsDefault { get; set; }
        public int AllowedNumberOfItems { get; set; }
        public bool? IsInfinityItems { get; set; }
        public double MonthPrice { get; set; }
        public double YearPrice { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<OfferPrice> OfferPrices { get; set; }
        public virtual ICollection<UserApplicationCash> UserApplicationCashes { get; set; }
    }
}
