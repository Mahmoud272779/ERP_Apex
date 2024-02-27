using App.Domain.Entities.Setup;
using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class AddItemPartRequest : IRequest<ResponseResult>
    {
        public int ItemId { get; set; }
        public int PartId { get; set; }
        public double Quantity { get; set; }
        public int UnitId { get; set; }
    }

    public class compositItem : AddItemPartRequest
    {
        public int indexOfItem { get; set; }
        public int invoiceId { get; set; }
    }
}
