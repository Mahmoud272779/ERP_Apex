using App.Domain.Models.Setup.ItemCard.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class GetItemCardRequest : IRequest<GetItemCardResponse>
    {
        public int ItemId { get; set; }
        public GetItemCardRequest(int ItemId)
        {
            this.ItemId = ItemId;
        }
    }
}
