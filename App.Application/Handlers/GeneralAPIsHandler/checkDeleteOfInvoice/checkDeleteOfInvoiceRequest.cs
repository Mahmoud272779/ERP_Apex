using App.Domain.Models;
using MediatR;

namespace App.Application.Handlers.GeneralAPIsHandler.checkDeleteOfInvoice
{
    public class checkDeleteOfInvoiceRequest : IRequest<bool>
    {
        public int InvoiceTypeId { get; set; }
        public bool IsAccredite { get; set; }
        public bool IsReturn { get; set; }
        public bool IsDeleted { get; set; }
        public string InvoiceType { get; set; }
        public int StoreId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceId { get; set; }
        public List<CalcQuantityRequest> items { get; set; }
    }
}
