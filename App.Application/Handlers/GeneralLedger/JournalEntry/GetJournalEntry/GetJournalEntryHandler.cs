using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetJournalEntryHandler : BusinessBase<GLJournalEntry>, IRequestHandler<GetJournalEntryRequest, IRepositoryActionResult>
    {
        private readonly IMediator _mediator;
        public GetJournalEntryHandler(IRepositoryActionResult repositoryActionResult, IMediator mediator) : base(repositoryActionResult)
        {
            _mediator = mediator;
        }

        public async Task<IRepositoryActionResult> Handle(GetJournalEntryRequest paramters, CancellationToken cancellationToken)
        {
            var servData = await _mediator.Send(new getJournalEntryServRequest { isPrint = false,PageNumber = paramters.PageNumber,PageSize = paramters.PageSize,Search = paramters.Search });
            var data = servData.data;
            var listCount = servData.journalEntry.Count();
            double MaxPageNumber = listCount / Convert.ToDouble(paramters.PageSize);
            var countofFilter = int.Parse(Math.Ceiling(MaxPageNumber).ToString());
            var response = new PaginationResponse()
            {
                Data = data,
                ListCount = listCount,
                PageNumber = paramters.PageNumber,
                PageSize = paramters.PageSize,
                TotalPages = countofFilter
            };
            return repositoryActionResult.GetRepositoryActionResult(response, RepositoryActionStatus.Ok);
        }
    }
}
