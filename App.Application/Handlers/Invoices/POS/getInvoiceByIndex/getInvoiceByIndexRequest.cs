using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.getInvoiceByIndex
{
    public class getInvoiceByIndexRequest : IRequest<ResponseResult>
    {
        public int invoiceTypeId { get; set; }
        public int index { get; set; }
        public int branchId { get; set; }
    }
}
