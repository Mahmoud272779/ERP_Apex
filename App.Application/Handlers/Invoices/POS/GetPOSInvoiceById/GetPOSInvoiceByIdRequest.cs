using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetPOSInvoiceById
{
    public class GetPOSInvoiceByIdRequest : IRequest<ResponseResult>
    {
        public int? InvoiceId { get; set; }
        public string? InvoiceCode { get; set; }
        public bool? ForIOS { get; set; }
    }
}
