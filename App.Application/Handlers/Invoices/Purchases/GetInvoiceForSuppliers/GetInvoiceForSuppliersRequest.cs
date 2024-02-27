using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetInvoiceForSuppliers
{
    public class GetInvoiceForSuppliersRequest : IRequest<ResponseResult>
    {
        public int InvoiceId { get; set; }
    }
}
