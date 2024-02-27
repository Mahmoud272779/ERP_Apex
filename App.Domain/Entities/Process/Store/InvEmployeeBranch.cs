using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store
{
    public class InvEmployeeBranch
    {

        public int EmployeeId { get; set; }
        public int BranchId { get; set; }
        public bool current { get; set; } = false;
        public virtual InvEmployees Employee { get; set; }
        public virtual GLBranch Branch { get; set; }
    }
}
