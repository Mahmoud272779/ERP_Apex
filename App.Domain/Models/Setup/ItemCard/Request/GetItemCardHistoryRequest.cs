using App.Domain.Models.Shared;
using MediatR;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetItemCardHistoryRequest:IRequest<ResponseResult>
    {
        public int ItemId { get; set; }

        public GetItemCardHistoryRequest(int ItemId)
        {
            this.ItemId = ItemId;
        }
    }
}
