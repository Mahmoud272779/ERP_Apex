using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.General
{
    public class SystemHistoryLogs
    {
        public int Id { get; set; }
        public int employeesId { get; set; }
        public int? BranchId { get; set; }
        public int TransactionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionLatinName { get; set; }
        public DateTime date { get; set; }
        public GLBranch GLBranch { get; set; }
        public InvEmployees employees { get; set; }
        public bool isTechnicalSupport { get; set; }
    }
}
