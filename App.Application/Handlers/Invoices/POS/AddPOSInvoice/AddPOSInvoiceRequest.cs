using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;

namespace App.Application.Handlers.Invoices.POS.AddPOSInvoice
{
    public class AddPOSInvoiceRequest : InvoiceMasterRequest,IRequest<ResponseResult>
    {

    }
}
