using App.Domain.Models.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLFinancialAccount : GeneralProperities
    {
        [NotMapped]
        public int? nextPageNumber;

        //private readonly ILazyLoader _lazyLoader;
        //public FinancialAccount()
        //{
        //}
        //public FinancialAccount(ILazyLoader lazyLoader)
        //{
        //    _lazyLoader = lazyLoader;
        ////}
        //private List<FinancialAccount> _FinancialAccounts;
        public int CurrencyId { get; set; }
        public string AccountCode { get; set; } = "";
        public string autoCoding { get; set; } = "";
        public int MainCode { get; set; }
        public int SubCode { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int FinalAccount { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double OpenningCredit { get; set; }
        public double OpenningDebit { get; set; }
        public string Notes { get; set; } = "";
        public int? ParentId { get; set; }
        public int? HasCostCenter { get; set; }
        public bool IsBlock { get; set; }
        public string BrowserName { get; set; } = "";
        [NotMapped]
        public bool hasChildren { get; set; }
        [NotMapped]
        public double? total { get; set; }

        public virtual GLCurrency Currency { get; set; }
        public IReadOnlyCollection<GLFinancialCost> financialCosts { get; set; }
        public IReadOnlyCollection<GLBank> Banks { get; set; }
        public IReadOnlyCollection<InvPersons> persons { get; set; }
        public IReadOnlyCollection<InvSalesMan> SalesMen { get; set; }
       // public IReadOnlyCollection<InvSalesMan> SalesMen { get; set; }
        public IReadOnlyCollection<GLFinancialAccountBranch> financialAccountBranches { get; set; }
        public IReadOnlyCollection<GLFinancialSetting> financialSettings { get; set; }
        public virtual GLFinancialAccount financialAccount { get; set; }
        public virtual IReadOnlyCollection<GLFinancialAccount> financialAccounts { get; set; }
       // public IReadOnlyCollection<GLJournalEntryDetailsAccounts> journalEntryDetailsAccounts { get; set; }
        public virtual IReadOnlyCollection<GLSafe> Treasuries { get; set; }
        public IList<GLJournalEntryDetails> GLJournalEntryDetails { get; set; }
        public IReadOnlyCollection<GlReciepts> reciepts { get; set; }
        public bool IsMain { get; set; }
        public ICollection<GLIntegrationSettings> gLIntegrationSettings { get; set; }

    }
}
