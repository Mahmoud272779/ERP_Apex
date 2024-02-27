using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLIntegrationSettings
    {
        public int Id { get; set; }
        public int linkingMethodId { get; set; }
        public int screenId { get; set; }
        public int GLFinancialAccountId { get; set; }
        public int GLBranchId { get; set; }
        public GLFinancialAccount GLFinancialAccount { get; set; }
        public GLBranch GLBranch { get; set; }
    }
}
