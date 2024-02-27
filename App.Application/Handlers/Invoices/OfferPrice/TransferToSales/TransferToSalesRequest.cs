using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.OfferPrice.TransferToSales
{
    public  class TransferToSalesRequest: IRequest<ResponseResult>
    {
        public int Id { get; set; }
    }
}
