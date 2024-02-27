using System;
using System.Collections;
using System.Collections.Generic;

namespace App.Domain.Entities.Process
{
  public  class InvFundsBanksSafesMaster 
    {
        public int DocumentId { get; set; }
        public int Code { get; set; }
        public DateTime DocDate { get; set; }
        public int? BankId { get; set; }
        public int? SafeId { get; set; }

        public string Notes { get; set; }
        public bool IsBank { get; set; }
        public bool IsSafe { get; set; }
        public bool isBlock { get; set; } = false;
        public int BranchId { get; set; }
        public ICollection<InvFundsBanksSafesDetails> FundsDetails_B_S { get; set; }
        public GLBank Bank { get; set; }
        public GLSafe Safe { get; set; }

    }
}
