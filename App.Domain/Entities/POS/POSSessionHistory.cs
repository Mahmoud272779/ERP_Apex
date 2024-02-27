using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.POS
{
    public class POSSessionHistory
    {
        public int Id { get; set; }
        [ForeignKey("POSSessionId")]
        public int POSSessionId { get; set; }
        public string actionAr { get; set; }
        public string actionEn { get; set; }
        public DateTime LastDate { get; set; }
        public string BrowserName { get; set; }

        [ForeignKey("employeesId")]
        public int employeesId { get; set; }
        public InvEmployees employees { get; set; }
        public POSSession POSSession { get; set; }
        public bool isTechnicalSupport { get; set; } = false;
    }
}
