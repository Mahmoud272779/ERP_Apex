using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.UpdatePOSInvoice
{
    public class UpdatePOSInvoiceRequest : UpdateInvoiceMasterRequest, IRequest<ResponseResult>
    {
       
    }
}
