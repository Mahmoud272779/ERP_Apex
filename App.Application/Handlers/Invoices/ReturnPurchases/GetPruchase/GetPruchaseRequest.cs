using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnPurchases.GetPruchase
{
    public class GetPruchaseRequest : IRequest<ResponseResult>
    {
        public string InvoiceCode { get; set; }
    }
}
