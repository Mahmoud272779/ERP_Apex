using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.BlockJournalEntry
{
    public class BlockJournalEntryHandler : BusinessBase<GLJournalEntry>,IRequestHandler<BlockJournalEntryReqeust, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;


        public BlockJournalEntryHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery, IRepositoryCommand<GLJournalEntry> journalEntryRepositoryCommand, IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, iUserInformation iUserInformation, ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.journalEntryRepositoryQuery = journalEntryRepositoryQuery;
            this.journalEntryRepositoryCommand = journalEntryRepositoryCommand;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<IRepositoryActionResult> Handle(BlockJournalEntryReqeust parameter, CancellationToken cancellationToken)
        {
            try
            {
                var count = parameter.Ids.Count();

                foreach (var item in parameter.Ids)
                {
                    var journalEntry = await journalEntryRepositoryQuery.GetByAsync(q => q.Id == item);

                    journalEntry.IsBlock = true;
                    var save = await journalEntryRepositoryCommand.UpdateAsyn(journalEntry);
                    if (save == true)
                    {
                        journalEntryHelper.HistoryJournalEntry(recHistoryRepositoryCommand, _iUserInformation, journalEntry.BranchId, journalEntry.Code, journalEntry.Id, journalEntry.BrowserName, "D", "D");

                    }
                }

                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteJournalEntry, count);
                return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Deleted, message: "Blocked Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
