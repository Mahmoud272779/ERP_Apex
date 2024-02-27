using MediatR;

namespace App.Application.Handlers.InvoicesHelper.CheckInvoiceExistance
{
    public class CheckInvoiceExistanceRequest : IRequest<ResponseResult>
    {
        public string invoiceType { get; set; } 
        public int InvoiceTypeId { get; set; }
    }
}
