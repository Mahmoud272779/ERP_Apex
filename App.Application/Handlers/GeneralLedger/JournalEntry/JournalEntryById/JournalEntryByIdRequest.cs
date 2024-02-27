using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class JournalEntryByIdRequest : IRequest<GetJournalEntryByID>
    {
        public int Id { get; set; }
        public int? docType { get; set; }
        public int? docId { get; set; }
    }
}
