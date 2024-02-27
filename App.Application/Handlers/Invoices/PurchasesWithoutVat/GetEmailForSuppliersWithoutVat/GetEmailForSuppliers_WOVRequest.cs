using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetEmailForSuppliers
{
    public class GetEmailForSuppliers_WOVRequest : IRequest<ResponseResult>
    {
        public int InvoiceId { get;set; }
    }
}
