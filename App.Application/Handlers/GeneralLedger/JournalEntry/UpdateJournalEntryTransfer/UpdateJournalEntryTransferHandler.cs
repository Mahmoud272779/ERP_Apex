using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.UpdateJournalEntryTransfer
{
    public class UpdateJournalEntryTransferHandler : BusinessBase<GLJournalEntry>, IRequestHandler<UpdateJournalEntryTransferRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;

        public UpdateJournalEntryTransferHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand, IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, iUserInformation iUserInformation) : base(repositoryActionResult)
        {
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.journalEntryRepositoryCommand = journalEntryRepositoryCommand;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
        }

        public async Task<IRepositoryActionResult> Handle(UpdateJournalEntryTransferRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in parameter.Id)
                {
                    var journalentry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == item);
                    journalentry.IsTransfer = (parameter.IsTransfer == 1 ? true : false);
                    await journalEntryRepositoryCommand.UpdateAsyn(journalentry);
                    journalEntryRepositoryCommand.SaveChanges();
                    journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, journalentry.BranchId, journalentry.Code, journalentry.Id, journalentry.BrowserName, journalentry.LastTransactionAction, journalentry.LastTransactionUser);
                }

                return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
