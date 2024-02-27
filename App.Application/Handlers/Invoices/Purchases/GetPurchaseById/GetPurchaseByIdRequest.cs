using MediatR;

namespace App.Application.Handlers.Purchases.GetPurchaseById
{
    public class GetPurchaseByIdRequest : IRequest<ResponseResult>
    {
        public int InvoiceId { get; set; }
        public bool isCopy { get; set; }
    }
}
