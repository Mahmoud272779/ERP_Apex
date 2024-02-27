using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class AddItemStoreRequest : IRequest<ResponseResult>
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal DemandLimit { get; set; }
    }
}
