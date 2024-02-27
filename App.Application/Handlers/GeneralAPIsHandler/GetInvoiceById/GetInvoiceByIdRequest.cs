using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById
{
    public class GetInvoiceByIdRequest : IRequest<ResponseResult>
    {
        public int InvoiceId { get; set; }
        public bool? isCopy { get; set; }
        public bool? ForIOS { get; set; } = false;
    }
}
