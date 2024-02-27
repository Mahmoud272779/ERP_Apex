using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLJournalEntry : AuditableEntity
    {
        public GLJournalEntry()
        {
            JournalEntryDetails = new List<GLJournalEntryDetails>();
        }
        public int Id { get; set; }
        public int Code { get; set; }
        public int CurrencyId { get; set; }
        public bool IsDeleted { get; set; }
        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public DateTime? FTDate { get; set; }
        public string Notes { get; set; }
        public bool IsBlock { get; set; }
        public bool IsTransfer { get; set; }
        public bool IsAccredit { get; set; } = true;
        public string BrowserName { get; set; }
        public bool Auto { get; set; }
        public bool IsCompined { get; set; } = false;
        public int? CompinedReceiptCode { get; set; } = 0;
        public int? ReceiptsId { get; set; }
        public int? InvoiceId { get; set; }
        public int? DocType{ get; set; }
        public DateTime? lastTimeProfitCalced { get; set; } //اخر وقت لاحتساب الربحيه للفاتورة
        public IReadOnlyCollection<GLJournalEntryDetails> JournalEntryDetails { get; set; }
        public IReadOnlyCollection<GLJournalEntryFiles> journalEntryFiles { get; set; }
      //  public IReadOnlyCollection<GLJournalEntryDetailsAccounts> JournalEntryDetailsAccounts { get; set; }
        public GLCurrency currency { get; set; }
    }
}
