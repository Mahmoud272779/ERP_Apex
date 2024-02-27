using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Restaurants;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Restaurants
{
    public class GetKitchensHistoryHandler : IRequestHandler<GetKitchensHistoryRequest, ResponseResult>
    {
        private readonly IHistory<KitchensHistory> history;

        public GetKitchensHistoryHandler(IHistory<KitchensHistory> history)
        {
            this.history = history;
        }

        public async Task<ResponseResult> Handle(GetKitchensHistoryRequest request, CancellationToken cancellationToken)
        {
            return await history.GetHistory(a => a.EntityId == request.Code);
        }
    }
}
