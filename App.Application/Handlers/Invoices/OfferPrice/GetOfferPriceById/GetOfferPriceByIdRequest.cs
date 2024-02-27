using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById
{
    public  class GetOfferPriceByIdRequest:GetInvoiceByIdRequest,IRequest<ResponseResult>
    {
    }
}
