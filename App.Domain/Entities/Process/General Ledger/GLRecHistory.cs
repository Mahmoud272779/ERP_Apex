using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLRecHistory: AuditableEntity
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int Code { get; set; }
        public DateTime Time { get; set; }
        public int EmployeeId { get; set; }
        public string Details { get; set; }
        public string Version { get; set; }
        public string MacAdress { get; set; }
        public string OS { get; set; }
        public int HistoryType { get; set; }
        public string LastAction { get; set; }
        public DateTime LastDate { get; set; }
        public string BrowserName { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
    }
}
