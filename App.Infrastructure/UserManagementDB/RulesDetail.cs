using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class RulesDetail
    {
        public string FormName { get; set; }
        public int RuleId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool? CanOpen { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanDelete { get; set; }
        public bool? CanPrint { get; set; }

        public virtual Rule Rule { get; set; }
    }
}
