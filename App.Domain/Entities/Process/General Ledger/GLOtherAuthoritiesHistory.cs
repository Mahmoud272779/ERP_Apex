using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLOtherAuthoritiesHistory 
    {
        public int Id { get; set; }
        public int AuthorityId { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int? FinancialAccountId { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public  string userName{ get; set; }
        public string LastAction { get; set; }
        public DateTime LastDate { get; set; }
        public string BrowserName { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
        public bool isTechnicalSupport { get; set; } = false;

    }
}
