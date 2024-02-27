using MediatR;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class removeStoreFundFromJournalDetialesRequest : IRequest<bool>
    {
        public int[] storeFundIds { get; set; }
        public int DocType {get;set;}
        public int journalId {get;set;}
    }
}
