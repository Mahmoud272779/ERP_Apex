using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices.POS.GetItemQuantity
{
    public class GetItemQuantityHandler : IRequestHandler<GetItemQuantityRequest, QuantityInStoreAndInvoice>
    {
        private readonly IGeneralAPIsService generalAPIsService;

        public GetItemQuantityHandler(IGeneralAPIsService generalAPIsService)
        {
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<QuantityInStoreAndInvoice> Handle(GetItemQuantityRequest request, CancellationToken cancellationToken)
        {
            return await generalAPIsService.CalculateItemQuantity(request.ItemId, request.UnitId, request.StoreId, request.ParentInvoiceType, request.ExpiryDate, request.IsExpiared, request.invoiceId, request.invoiceDate, (int)Domain.Enums.Enums.DocumentType.POS, null);
        }
    }
}
