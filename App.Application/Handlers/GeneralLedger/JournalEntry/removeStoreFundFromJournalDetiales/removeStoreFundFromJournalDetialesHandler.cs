using MediatR;

using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class removeStoreFundFromJournalDetialesHandler : IRequestHandler<removeStoreFundFromJournalDetialesRequest, bool>
    {
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand;
        private readonly iUserInformation _iUserInformation;

        public removeStoreFundFromJournalDetialesHandler(IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery, IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand, iUserInformation iUserInformation)
        {
            this.journalEntryDetailsRepositoryQuery = journalEntryDetailsRepositoryQuery;
            this.journalEntryDetailsRepositoryCommand = journalEntryDetailsRepositoryCommand;
            _iUserInformation = iUserInformation;
        }


        public async Task<bool> Handle(removeStoreFundFromJournalDetialesRequest request, CancellationToken cancellationToken)
        {
            var _storeFundJournalEntry = journalEntryDetailsRepositoryQuery.TableNoTracking
                .Where(c=> c.JournalEntryId == request.journalId)
                .Where(x => x.DocType == request.DocType);

            var storeFundJournalEntry = _storeFundJournalEntry
                .Where(x => request.storeFundIds.Contains(x.StoreFundId ?? 0));

            if (storeFundJournalEntry.Any())
            {
                journalEntryDetailsRepositoryCommand.RemoveRange(storeFundJournalEntry);
                var deleted = await journalEntryDetailsRepositoryCommand.SaveAsync();
                var MainAccount = _storeFundJournalEntry.Where(x => x.StoreFundId == null).FirstOrDefault();
                var balance = _storeFundJournalEntry.Where(x => x.StoreFundId != null).Sum(x => x.Credit - x.Debit);
                if (balance == 0)
                {
                    journalEntryDetailsRepositoryCommand.Remove(MainAccount);
                    await journalEntryDetailsRepositoryCommand.SaveAsync();
                    return deleted;

                }
                MainAccount.Credit = balance < 0 ? (balance * -1) : 0;
                MainAccount.Debit = balance < 0 ? 0 : balance;
                await journalEntryDetailsRepositoryCommand.UpdateAsyn(MainAccount);
                return deleted;
            }

            return true;
        }
    }
}
