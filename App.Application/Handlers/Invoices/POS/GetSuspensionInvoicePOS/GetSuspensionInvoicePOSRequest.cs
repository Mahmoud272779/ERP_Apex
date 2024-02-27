using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOS
{
    public class GetSuspensionInvoicePOSRequest : IRequest<ResponseResult>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
