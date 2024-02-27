using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetAllJournalEntryHistoryRequest : IRequest<ResponseResult>
    {
        public int JournalEntryId{ get; set; }
    }
}
