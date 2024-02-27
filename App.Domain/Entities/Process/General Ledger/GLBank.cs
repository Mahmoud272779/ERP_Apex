using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLBank
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int Code { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public int? FinancialAccountId { get; set; }
        public int Status { get; set; }
        public string ArabicAddress { get; set; }
        public string LatinAddress { get; set; }
        public string Notes { get; set; }
        public string BrowserName { get; set; }
        public string  Website { get; set; }
        public bool CanDelete { get; set; } = false;
        public virtual GLFinancialAccount FinancialAccount { get; set; }
        public virtual ICollection<GlReciepts> reciept { get; set; }
        public virtual ICollection<GLBankBranch> BankBranches { get; set; }
        public virtual ICollection<InvPaymentMethods> PaymentMethod { get; set; }
        public virtual ICollection<InvFundsBanksSafesMaster> FundsBanksSafes { get; set; }
        public virtual ICollection<OtherSettingsBanks> OtherSettingsBanks { get; set; }

    }
}
