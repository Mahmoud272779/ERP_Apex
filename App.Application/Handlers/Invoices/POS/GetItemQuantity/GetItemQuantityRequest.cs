using MediatR;

namespace App.Application.Handlers.Invoices.POS.GetItemQuantity
{
    public class GetItemQuantityRequest : IRequest<QuantityInStoreAndInvoice>
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public int StoreId { get; set; }
        public string? ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int invoiceId { get; set; }
        public DateTime invoiceDate { get; set; }
    }
}
