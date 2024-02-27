using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class CheckItemNameRequest : IRequest<ResponseResult>
    {
        public string ItemName { get; set; }
        public int? ItemId { get; set; }
        public CheckItemNameRequest(string ItemName , int? ItemId)
        {
            this.ItemName = ItemName;
            this.ItemId = ItemId;
        }
    }
}
