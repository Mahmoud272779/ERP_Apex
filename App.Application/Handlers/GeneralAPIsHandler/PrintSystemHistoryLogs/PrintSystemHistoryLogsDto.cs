using App.Domain.Entities.Process.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.PrintSystemHistoryLogs
{
    public class PrintSystemHistoryLogsDto
    {
        public int Id { get; set; }
        public int employeesId { get; set; }
        public int? BranchId { get; set; }
        public int TransactionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionLatinName { get; set; }
        public DateTime date { get; set; }

        public bool isTechnicalSupport { get; set; }
    }
}
