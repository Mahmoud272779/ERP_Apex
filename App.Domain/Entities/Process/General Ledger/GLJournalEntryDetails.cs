using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLJournalEntryDetails
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int? FinancialAccountId { get; set; }
        public string? FinancialCode { get; set; }
        public string? FinancialName { get; set; }
        public int? CostCenterId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public bool isCostSales { get; set; } = false;
        public int ReceiptsMainCode { get; set; } 
        public bool isStoreFund { get; set; } = false; 
        public int? StoreFundId { get; set; } // ==> for safe and banks
        public int? DocType { get; set; }
        public virtual GLJournalEntry journalEntry { get; set; }
        public GLFinancialAccount GLFinancialAccount { get; set; }
    }
}
