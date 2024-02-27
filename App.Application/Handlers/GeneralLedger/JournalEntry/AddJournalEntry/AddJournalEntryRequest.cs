using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class AddJournalEntryRequest : JournalEntryParameter,IRequest<IRepositoryActionResult>
    {
    }
}
