using App.Domain.Models.Shared;
using MediatR;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class DeleteItemUnitRequest : IRequest<ResponseResult>
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
    }
}
