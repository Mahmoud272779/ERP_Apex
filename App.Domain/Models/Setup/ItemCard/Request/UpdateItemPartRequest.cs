using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class UpdateItemPartRequest : IRequest<ResponseResult>
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public double Quantity { get; set; }
        public int UnitId { get; set; }
    }
}
