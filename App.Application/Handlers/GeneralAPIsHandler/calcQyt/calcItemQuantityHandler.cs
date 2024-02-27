using App.Application.Helpers.CalcItemQuantity;
using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.calcQyt
{
    public class calcItemQuantityHandler : IRequestHandler<calcItemQuantityRequest, QuantityInStoreAndInvoice>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;
        public calcItemQuantityHandler(IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery, IRepositoryQuery<InvGeneralSettings> generalSettings, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery)
        {
            this.itemCardMasterRepository = itemCardMasterRepository;
            this.itemCardPartsQuery = itemCardPartsQuery;
            this.itemUnitsQuery = itemUnitsQuery;
            GeneralSettings = generalSettings;
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            this.invoiceMasterQuery = invoiceMasterQuery;
        }


        public async Task<QuantityInStoreAndInvoice> Handle(calcItemQuantityRequest request, CancellationToken cancellationToken)
        {
            return await CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
            {
                itemCardMasterRepository = itemCardMasterRepository,
                itemCardPartsQuery = itemCardPartsQuery,
                itemUnitsQuery = itemUnitsQuery,
                GeneralSettings = GeneralSettings,
                invoiceDetailsQuery = invoiceDetailsQuery,
                invoiceMasterQuery = invoiceMasterQuery,
                ItemId = request.ItemId,
                ExpiryDate = request.ExpiryDate,
                invoiceDate = request.invoiceDate,
                invoiceId = request.invoiceId ,
                invoiceTypeId = request.invoiceTypeId,
                IsExpiared = request.IsExpiared,
                items = request.items,
                itemTypeId = request.itemTypeId,
                ParentInvoiceType = request.ParentInvoiceType,
                StoreId = request.StoreId,
                UnitId = request.UnitId,
                currentQuantity=request.currentQuantity
            });
        }
    }
}
