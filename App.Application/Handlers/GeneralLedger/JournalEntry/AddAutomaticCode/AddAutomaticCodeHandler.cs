using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class AddAutomaticCodeHandler : IRequestHandler<AddAutomaticCodeRequest, string>
    {
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;

        public AddAutomaticCodeHandler(IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery)
        {
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
        }
        public async Task<string> Handle(AddAutomaticCodeRequest request, CancellationToken cancellationToken)
        {
            var code = journalEntryRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = journalEntryRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
                int codee = (Convert.ToInt32(code2.Code));
                if (codee == 0)
                {
                }
                var NewCode = codee + 1;
                return NewCode.ToString();

            }
            else
            {
                var NewCode = 1;
                return NewCode.ToString();

            }
        }
    }
}
