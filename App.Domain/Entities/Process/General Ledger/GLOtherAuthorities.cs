using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLOtherAuthorities
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int Code { get; set; }
        public int? FinancialAccountId { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
        public ICollection<GlReciepts> reciept { get; set; }
        public int BranchId { get; set; }
        public virtual GLBranch Branch { get; set; }


    }
}
