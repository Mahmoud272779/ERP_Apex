using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using MediatR;
using System.Collections.Generic;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetAllItemStoreRequest : PaginationVM, IRequest<List<GetAllItemStoreResponse>>
    {
        public int ItemId { get; set; }
        public GetAllItemStoreRequest(int pageIndex, int pageSize, int itemId)
        {
            PageNumber = pageIndex;
            PageSize = pageSize;
            ItemId = itemId;
        }
    }
}
