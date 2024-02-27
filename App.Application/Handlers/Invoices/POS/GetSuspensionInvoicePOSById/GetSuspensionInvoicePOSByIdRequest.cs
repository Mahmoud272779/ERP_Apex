using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOSById
{
    public class GetSuspensionInvoicePOSByIdRequest : IRequest<ResponseResult>
    {
        public int Id { get; set; }
    }
}
