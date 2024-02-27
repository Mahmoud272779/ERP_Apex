using App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.UpdateOfferPriceStatus
{
    public class UpdateOfferPriceStatusRequest:IRequest<ResponseResult>
    {
        public int offerPriceId { get; set; }
    }
}
