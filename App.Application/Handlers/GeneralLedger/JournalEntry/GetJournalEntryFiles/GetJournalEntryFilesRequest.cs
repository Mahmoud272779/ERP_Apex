using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetJournalEntryFilesRequest : IRequest<IRepositoryActionResult>
    {
        public int JournalEntryId { get; set; }
        public int JournalEntryDraftId { get; set; }
    }
}
