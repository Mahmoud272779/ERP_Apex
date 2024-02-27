using App.Domain.Common;
using App.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLJournalEntryDraft: AuditableEntity
    {
        public GLJournalEntryDraft()
        {
            journalEntryDraftDetails = new List<GLJournalEntryDraftDetails>();
        }
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public int CurrencyId { get; set; }
        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public DateTime? FTDate { get; set; }
        public string Notes { get; set; }
        public bool IsBlock { get; set; }
        public IReadOnlyCollection<GLJournalEntryDraftDetails> journalEntryDraftDetails { get; set; }
        public IReadOnlyCollection<GLJournalEntryFilesDraft> journalEntryFilesDrafts { get; set; }
    }
}
