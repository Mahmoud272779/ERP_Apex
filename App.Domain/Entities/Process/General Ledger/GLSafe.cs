using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLSafe
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int Code { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
        public string BrowserName { get; set; }
        public bool CanDelete { get; set; }
        public virtual GLFinancialAccount financialAccount { get; set; }
        public int BranchId { get; set; }
        public virtual GLBranch Branch { get; set; }
        public IReadOnlyCollection<GlReciepts> reciept { get; set; }
        public ICollection<InvPaymentMethods> PaymentMethod { get; set; }
        public ICollection<InvFundsBanksSafesMaster> FundsBanksSafes { get; set; }
        public ICollection<OtherSettingsSafes> OtherSettingsSafes { get; set; }


    }
}
