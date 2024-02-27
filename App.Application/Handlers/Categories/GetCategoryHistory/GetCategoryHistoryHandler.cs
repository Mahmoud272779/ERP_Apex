using App.Application.Helpers.Service_helper.History;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetCategoryHistoryHandler : IRequestHandler<GetCategoryHistoryRequest, ResponseResult>
    {
        private readonly IHistory<InvCategoriesHistory> history;

        public GetCategoryHistoryHandler(IHistory<InvCategoriesHistory> history)
        {
            this.history = history;
        }

        public async Task<ResponseResult> Handle(GetCategoryHistoryRequest request, CancellationToken cancellationToken)
        {
            return await history.GetHistory(a => a.EntityId == request.CategoryId);
        }
    }
}
