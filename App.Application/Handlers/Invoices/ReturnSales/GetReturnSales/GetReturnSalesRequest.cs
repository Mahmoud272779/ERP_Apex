using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnSales.GetReturnSales
{
    public class GetReturnSalesRequest : IRequest<ResponseResult>
    {
        public string InvoiceCode { get; set; }
    }
}
