using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvPersons_Branches
    {
        //public int Id { get; set; }
        public int BranchId { get; set; }
        public int PersonId { get; set; }
        public virtual InvPersons Person { get; set; }
        public virtual GLBranch Branch { get; set; }
    }
}
