using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetJournalEntryHistoryRequest : IRequest<ResponseResult>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
    }
}
