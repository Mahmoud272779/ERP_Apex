using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLBankBranch
    {
        public int BranchId { get; set; }
        public int BankId { get; set; }
        public virtual GLBranch Branch { get; set; }
        public virtual GLBank Bank { get; set; }
    }
}
