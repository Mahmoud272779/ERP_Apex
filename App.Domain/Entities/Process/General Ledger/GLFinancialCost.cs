using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLFinancialCost
    {
        public int FinancialAccountId { get; set; }
        public int CostCenterId { get; set; }
        public virtual GLCostCenter CostCenter { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
    }
}
