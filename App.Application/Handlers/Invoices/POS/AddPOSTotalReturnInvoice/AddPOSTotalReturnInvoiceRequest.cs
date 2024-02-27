using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.AddPOSTotalReturnInvoice
{
    public class AddPOSTotalReturnInvoiceRequest : IRequest<ResponseResult>
    {
        public int Id {  get; set; }
    }
}
