using System.Globalization;
using System;
namespace App.Domain.Common
{
    public class AuditableEntity
    {
        public int BranchId { get; set; }
        public string LastTransactionAction { get; set; }
        public string AddTransactionUser { get; set; }
        public string AddTransactionDate { get; set; }
        public string LastTransactionUser { get; set; }
        public string LastTransactionDate { get; set; }
        public string DeleteTransactionUser { get; set; }
        public string DeleteTransactionDate { get; set; }
        public string  LastTransactionUserAr { get; set; }
        public bool isTechnicalSupport { get; set; } = false;
    }
}
