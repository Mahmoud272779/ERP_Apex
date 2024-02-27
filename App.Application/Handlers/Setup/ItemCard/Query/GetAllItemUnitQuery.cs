using App.Domain.Entities.Setup;
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

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class GetAllItemUnitQuery : BaseClass, IRequestHandler<GetAllItemUnitRequest, List<GetAllItemUnitResponse>>
    {
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery;
        public GetAllItemUnitQuery(IHttpContextAccessor httpContextAccessor, IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery) : base(httpContextAccessor)
        {
            this.itemUnitRepositoryQuery = itemUnitRepositoryQuery;
        }
        public async Task<List<GetAllItemUnitResponse>> Handle(GetAllItemUnitRequest request, CancellationToken cancellationToken)
        {
            List<GetAllItemUnitResponse> responses = new List<GetAllItemUnitResponse>();
            var res = await itemUnitRepositoryQuery.GetWithPaging(request.PageNumber, request.PageSize,e=> e.ItemId == request.ItemId, e => e.Unit, s => s.Item);
            Mapping.Mapper.Map(res, responses, typeof(List<InvStpItemCardUnit>), typeof(List<GetAllItemUnitResponse>));
            return responses;
        }
    }
}
