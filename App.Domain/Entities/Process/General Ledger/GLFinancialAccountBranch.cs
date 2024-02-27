using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLFinancialAccountBranch
    {
        public int BranchId { get; set; }
        public int FinancialAccountId { get; set; }
        public virtual GLBranch Branch { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
    }
}
