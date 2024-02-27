using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class GeneralSetting
    {
        public int Id { get; set; }
        public bool AutomaticCoding { get; set; }
        public int? MainCode { get; set; }
        public int? SubCode { get; set; }
        public int BranchId { get; set; }
        public string LastTransactionAction { get; set; }
        public string AddTransactionUser { get; set; }
        public string AddTransactionDate { get; set; }
        public string LastTransactionUser { get; set; }
        public string LastTransactionDate { get; set; }
        public string DeleteTransactionUser { get; set; }
        public string DeleteTransactionDate { get; set; }
    }
}
