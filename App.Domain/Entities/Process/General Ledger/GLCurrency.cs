using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLCurrency 
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int Code { get; set; }
        public string CoinsAr { get; set; }
        public string CoinsEn { get; set; }
        public string AbbrAr { get; set; }
        public string AbbrEn { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public double Factor { get; set; }
        public bool IsDefault { get; set; }
        public bool CanDelete { get; set; }
        public string  LastUpdate { get; set; }
        public string CurrancySymbol { get; set; }
        public string BrowserName { get; set; }

        
        public IReadOnlyCollection<GLFinancialAccount> financialAccounts { get; set; }
        public ICollection<GLJournalEntry> journalEntry { get; set; }

    }
}
