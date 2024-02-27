using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store
{
   public  class InvSalesMan_Branches
    {
        public int BranchId { get; set; }
        public int SalesManId { get; set; }
        public virtual InvSalesMan SalesMan { get; set; }
        public virtual GLBranch Branch { get; set; }
    }
}
