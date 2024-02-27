using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Common
{
    public class GLJournalEntryDraftDetails
    {
        public int Id { get; set; }
        public int JournalEntryDraftId { get; set; }
        public int? FinancialAccountId { get; set; }
        public int? CostCenterId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public virtual GLJournalEntryDraft JournalEntryDraft { get; set; }
    }
}
