using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetReturnPOS
{
    public class GetReturnPOSRequest : IRequest<ResponseResult>
    {
        public string InvoiceCode { get; set; }
        public bool? ForIOS { get; set; }
    }
}
