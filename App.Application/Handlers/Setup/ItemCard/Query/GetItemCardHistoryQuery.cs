using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class GetItemCardHistoryQuery : IRequestHandler<GetItemCardHistoryRequest, ResponseResult>
    {
        
        private readonly IHistory<InvStpItemCardHistory> history;

        public GetItemCardHistoryQuery(IHistory<InvStpItemCardHistory> history)
        {
            this.history = history;
        }

        public async Task<ResponseResult> Handle(GetItemCardHistoryRequest request, CancellationToken cancellationToken)
        {
            return await history.GetHistory(a=>a.EntityId== request.ItemId);
        }
    }
}
