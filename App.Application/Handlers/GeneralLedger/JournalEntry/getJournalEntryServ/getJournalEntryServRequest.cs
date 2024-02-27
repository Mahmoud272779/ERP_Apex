using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class getJournalEntryServRequest : PageParameterJournalEntries, IRequest<getAllJournalEntryResponse>
    {
        public bool isPrint { get; set; } = false;
    }
}
