using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Rule
    {
        public Rule()
        {
            Accounts = new HashSet<Account>();
            RulesDetails = new HashSet<RulesDetail>();
        }

        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool? IsActive { get; set; }
        public bool CanDelete { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<RulesDetail> RulesDetails { get; set; }
    }
}
