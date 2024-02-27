using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Account
    {
        public Account()
        {
            BroadCastMasters = new HashSet<BroadCastMaster>();
            OffPriceHistories = new HashSet<OffPriceHistory>();
            SigninLogs = new HashSet<SigninLog>();
            UserApplicationCashAccAccounts = new HashSet<UserApplicationCash>();
            UserApplicationCashAccManagers = new HashSet<UserApplicationCash>();
            UserApplicationCashAccTeches = new HashSet<UserApplicationCash>();
        }

        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool? AllowTrialperiod { get; set; }
        public bool? BindTrialAccounts { get; set; }
        public bool? AccreditAppsAccounts { get; set; }
        public bool? AccreditAppsTechnicals { get; set; }
        public bool? ConfirmApps { get; set; }
        public int RuleId { get; set; }
        public int? EmployeesId { get; set; }
        public bool? CancelOfferPrice { get; set; }
        public bool? SendOfferPriceToSalesman { get; set; }
        public bool? SendOfferPriceToSubRequest { get; set; }

        public virtual Employee Employees { get; set; }
        public virtual Rule Rule { get; set; }
        public virtual ICollection<BroadCastMaster> BroadCastMasters { get; set; }
        public virtual ICollection<OffPriceHistory> OffPriceHistories { get; set; }
        public virtual ICollection<SigninLog> SigninLogs { get; set; }
        public virtual ICollection<UserApplicationCash> UserApplicationCashAccAccounts { get; set; }
        public virtual ICollection<UserApplicationCash> UserApplicationCashAccManagers { get; set; }
        public virtual ICollection<UserApplicationCash> UserApplicationCashAccTeches { get; set; }
    }
}
