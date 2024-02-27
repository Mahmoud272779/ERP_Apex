using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class RemoveJournalEntryFilesHandler : BusinessBase<GLJournalEntry>, IRequestHandler<RemoveJournalEntryFilesRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLJournalEntryFiles> journalEntryFilesRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand;

        public RemoveJournalEntryFilesHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLJournalEntryFiles> journalEntryFilesRepositoryQuery, IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand) : base(repositoryActionResult)
        {
            this.journalEntryFilesRepositoryQuery = journalEntryFilesRepositoryQuery;
            this.journalEntryFilesRepositoryCommand = journalEntryFilesRepositoryCommand;
        }
        public async Task<IRepositoryActionResult> Handle(RemoveJournalEntryFilesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var journal = await journalEntryFilesRepositoryQuery.GetByAsync(q => q.Id == request.Id);
                journalEntryFilesRepositoryCommand.Remove(journal);
                await journalEntryFilesRepositoryCommand.SaveAsync();
                return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Created, message: "Deleted Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
