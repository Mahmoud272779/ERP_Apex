using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class GetJournalEntryByIdHandler : BusinessBase<GLJournalEntry>,IRequestHandler<GetJournalEntryByIdRequest, IRepositoryActionResult>
    {
        private readonly IMediator _mediator;
        public GetJournalEntryByIdHandler(IRepositoryActionResult repositoryActionResult, IMediator mediator) : base(repositoryActionResult)
        {
            _mediator = mediator;
        }

        public async Task<IRepositoryActionResult> Handle(GetJournalEntryByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _mediator.Send(new JournalEntryByIdRequest { Id = request.Id,docType = request.docType,docId = request.docId});
                return repositoryActionResult.GetRepositoryActionResult(res, RepositoryActionStatus.Ok, message: "Ok");

            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
