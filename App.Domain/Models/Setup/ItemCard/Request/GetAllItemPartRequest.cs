using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetAllItemPartRequest : PaginationVM,IRequest<List<GetAllItemPartResponse>>
    {
        public int ItemId { get; set; }
        public GetAllItemPartRequest(int? pageIndex, int? pageSize, int itemId)
        {
            PageNumber = pageIndex;
            PageSize = pageSize;
            ItemId = itemId;
        }
    }
}
