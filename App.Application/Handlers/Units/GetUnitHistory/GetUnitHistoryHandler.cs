using App.Application.Helpers.Service_helper.History;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class GetKitchensHistoryHandler : IRequestHandler<GetKitchensHistoryRequest, ResponseResult>
    {
        private readonly IHistory<InvUnitsHistory> history;

        public GetKitchensHistoryHandler(IHistory<InvUnitsHistory> history)
        {
            this.history = history;
        }

        public async Task<ResponseResult> Handle(GetKitchensHistoryRequest request, CancellationToken cancellationToken)
        {
            return await history.GetHistory(a => a.EntityId == request.Code);
        }
    }
}
