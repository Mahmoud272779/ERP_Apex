using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.sales
{
    public class GetSalesByIdRequest : IRequest<ResponseResult>
    {
        public int InvoiceId { get; set; }
        public bool isCopy { get; set; }
    }
}
