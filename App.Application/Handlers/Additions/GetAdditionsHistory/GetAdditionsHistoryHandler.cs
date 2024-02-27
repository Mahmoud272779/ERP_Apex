using App.Application.Handlers.Units;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.GetAdditionsHistory
{
    public  class GetAdditionsHistoryHandler : IRequestHandler<GetAdditionsHistoryRequest, ResponseResult>
    {
        private readonly IHistory<InvPurchasesAdditionalCostsHistory> history;

        public GetAdditionsHistoryHandler(IHistory<InvPurchasesAdditionalCostsHistory> history)
        {
            this.history = history;
        }

        public async Task<ResponseResult> Handle(GetAdditionsHistoryRequest request, CancellationToken cancellationToken)
        {
            return await history.GetHistory(a => a.EntityId == request.Code);
        }
    }
}
