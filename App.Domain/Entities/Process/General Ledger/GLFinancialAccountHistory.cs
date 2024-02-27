using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLFinancialAccountHistory : GeneralProperities
    {
        
        public int CurrencyId { get; set; }
        public string AccountCode { get; set; }
        public int MainCode { get; set; }
        public int SubCode { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int FinalAccount { get; set; }
        public string LastAction { get; set; }
        public DateTime LastDate { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
        public int? HasCostCenter { get; set; }
        public string BrowserName { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
    }
}
