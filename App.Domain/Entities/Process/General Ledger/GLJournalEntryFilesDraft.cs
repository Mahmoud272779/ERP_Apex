using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLJournalEntryFilesDraft
    {
        public int Id { get; set; }
        public string File { get; set; }
        public int JournalEntryDraftId { get; set; }
        public virtual GLJournalEntryDraft journalEntryDraft { get; set; }
    }
}
