using App.Domain.Entities.Setup;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
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
    public class GetAllItemStoreQuery : BaseClass, IRequestHandler<GetAllItemStoreRequest, List<GetAllItemStoreResponse>>
    {
        private readonly IRepositoryQuery<InvStpItemCardStores> itemStoreRepositoryQuery;
        public GetAllItemStoreQuery(IHttpContextAccessor httpContextAccessor, IRepositoryQuery<InvStpItemCardStores> itemStoreRepositoryQuery) : base(httpContextAccessor)
        {
            this.itemStoreRepositoryQuery = itemStoreRepositoryQuery;
        }

        public async Task<List<GetAllItemStoreResponse>> Handle(GetAllItemStoreRequest request, CancellationToken cancellationToken)
        {
            List<GetAllItemStoreResponse> responses = new List<GetAllItemStoreResponse>();
            var res =await itemStoreRepositoryQuery.GetWithPaging(request.PageNumber, request.PageSize,e=> e.ItemId == request.ItemId, e => e.Store, s => s.Item);
            Mapping.Mapper.Map(res, responses, typeof(List<InvStpItemCardStores>), typeof(List<GetAllItemStoreResponse>));
            return responses;
        }
    }
}
