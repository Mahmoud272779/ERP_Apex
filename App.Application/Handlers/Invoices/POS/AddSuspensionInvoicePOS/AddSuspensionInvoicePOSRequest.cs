using App.Domain.Models.Request.POS;
using MediatR;

namespace App.Application.Handlers.Invoices.POS.AddSuspensionInvoicePOS
{
    public class AddSuspensionInvoicePOSRequest : InvoiceSuspensionRequest,IRequest<ResponseResult>
    {
    }
}
