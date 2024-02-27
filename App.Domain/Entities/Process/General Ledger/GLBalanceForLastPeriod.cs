using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLBalanceForLastPeriod: AuditableEntity
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public double TotalIncomeList { get; set; }
    }
}
