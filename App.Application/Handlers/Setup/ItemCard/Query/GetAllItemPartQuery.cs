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
    public class GetAllItemPartQuery : BaseClass, IRequestHandler<GetAllItemPartRequest, List<GetAllItemPartResponse>>
    {
        private readonly IRepositoryQuery<InvStpItemCardParts> itemPartRepositoryQuery;
        public GetAllItemPartQuery(IHttpContextAccessor httpContextAccessor, IRepositoryQuery<InvStpItemCardParts> itemPartRepositoryQuery) : base(httpContextAccessor)
        {
            this.itemPartRepositoryQuery = itemPartRepositoryQuery;
        }
        public async Task<List<GetAllItemPartResponse>> Handle(GetAllItemPartRequest request, CancellationToken cancellationToken)
        {
            List<GetAllItemPartResponse> responses = new List<GetAllItemPartResponse>();
            var res = await itemPartRepositoryQuery.GetWithPaging(request.PageNumber, request.PageSize, e => e.ItemId == request.ItemId, e => e.PartDetails, s => s.Unit);
            Mapping.Mapper.Map(res, responses, typeof(List<InvStpItemCardParts>), typeof(List<GetAllItemPartResponse>));
            return responses;
        }
    }
}
