using App.Domain.Models.Request.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetAllPOSInvoices
{
    public class GetAllPOSInvoicesRequest : POSReturnInvoiceSearchDTO, IRequest<ResponseResult>
    {
    }
}
