using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class AddJournalEntryFilesRequest : JournalEntryFilesDto,IRequest<IRepositoryActionResult>
    {
    }
}
