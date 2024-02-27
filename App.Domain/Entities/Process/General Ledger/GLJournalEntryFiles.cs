using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLJournalEntryFiles
    {
        public int Id { get; set; }
        public string File { get; set; }
        public int JournalEntryId { get; set; }
        public virtual GLJournalEntry JournalEntry { get; set; }
    }
}
