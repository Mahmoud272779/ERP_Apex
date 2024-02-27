using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Branch
    {
        public int BranchId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string AddressEn { get; set; }
        public string AddressAr { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? ManagerId { get; set; }
        public string LastTransactionAction { get; set; }
        public string AddTransactionUser { get; set; }
        public string AddTransactionDate { get; set; }
        public string LastTransactionUser { get; set; }
        public string LastTransactionDate { get; set; }
        public string DeleteTransactionUser { get; set; }
        public string DeleteTransactionDate { get; set; }
    }
}
