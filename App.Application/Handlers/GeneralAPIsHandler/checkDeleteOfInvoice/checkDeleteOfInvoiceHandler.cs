using App.Application.Helpers.CalcItemQuantity;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Response.Store;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Threading;

namespace App.Application.Handlers.GeneralAPIsHandler.checkDeleteOfInvoice
{
    public class checkDeleteOfInvoiceHandler : IRequestHandler<checkDeleteOfInvoiceRequest, bool>
    {
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;


        public checkDeleteOfInvoiceHandler(IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery, IRepositoryQuery<InvSerialTransaction> serialTransactionQuery, IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvGeneralSettings> generalSettings)
        {
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            this.serialTransactionQuery = serialTransactionQuery;
            this.itemCardMasterRepository = itemCardMasterRepository;
            this.itemCardPartsQuery = itemCardPartsQuery;
            this.itemUnitsQuery = itemUnitsQuery;
            this.invoiceMasterQuery = invoiceMasterQuery;
            GeneralSettings = generalSettings;
        }

        public checkDeleteOfInvoiceHandler(IRepositoryQuery<InvSerialTransaction> serialTransactionQuery)
        {
            this.serialTransactionQuery = serialTransactionQuery;
        }

        public async Task<bool> Handle(checkDeleteOfInvoiceRequest request, CancellationToken cancellationToken)
        {
            bool CanDelete = true;
            // incase of aaccredited
            if (((Lists.salesInvoicesList.Contains(request.InvoiceTypeId) ||
                Lists.purchasesInvoicesList.Contains(request.InvoiceTypeId))
                && request.IsAccredite)
                 || request.IsReturn || request.IsDeleted)
            {
                CanDelete = false;
            }
            // check serials
            else if (Lists.salesInvoicesList.Contains(request.InvoiceTypeId) || Lists.POSInvoicesList.Contains(request.InvoiceTypeId)
            || Lists.ExtractPermissionInvoicesList.Contains(request.InvoiceTypeId))
            {

                List<string> serialsList = serialTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == request.InvoiceType
                    || a.ExtractInvoice == request.InvoiceType).Select(a => a.SerialNumber).ToList();
                if (serialsList.Count > 0)
                {
                    //   var serialExist = serialsService.checkSerialBeforeSave(false ,null,null, InvoiceTypeId, serialsList);
                    var serialsFromDb = serialTransactionQuery.TableNoTracking.Where(a => serialsList.Contains(a.SerialNumber)
                && a.AddedInvoice != null && a.ExtractInvoice == null && a.IsDeleted == false).ToList();

                    if (serialsFromDb.Count() > 0)
                    {
                        CanDelete = false;
                        //      InvoiceDto.CanEdit = false;
                    }
                    else
                        CanDelete = true;

                }

            }
            else if (Lists.purchasesInvoicesList.Contains(request.InvoiceTypeId) || Lists.AddPermissionInvoicesList.Contains(request.InvoiceTypeId)
                || Lists.ItemsFundList.Contains(request.InvoiceTypeId))
            {
                // استرجاع بدون رصيد
                var PurchasesReturnWithoutQuantity = GeneralSettings.TableNoTracking.Select(a => a.Purchases_ReturnWithoutQuantity).First();
                var details = invoiceDetailsQuery.TableNoTracking
                .Where(q => q.InvoiceId == request.InvoiceId && q.ItemTypeId != (int)ItemTypes.Service && q.ItemTypeId != (int)ItemTypes.Composite
                         && ((PurchasesReturnWithoutQuantity == true && q.ItemTypeId != (int)ItemTypes.Expiary) ? false : q.Quantity >

                         CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
                         {
                             invoiceId = 0,
                             ItemId = q.ItemId,
                             UnitId = q.UnitId,
                             StoreId = request.StoreId,
                             ParentInvoiceType = "",
                             ExpiryDate = q.ExpireDate,
                             IsExpiared = false,
                             invoiceTypeId = 0,
                             invoiceDate = request.InvoiceDate,
                             items = null,
                             itemTypeId = q.ItemId,
                             GeneralSettings = GeneralSettings,
                             invoiceDetailsQuery = invoiceDetailsQuery,
                             invoiceMasterQuery = invoiceMasterQuery,
                             itemCardMasterRepository = itemCardMasterRepository,
                             itemCardPartsQuery = itemCardPartsQuery,
                             itemUnitsQuery = itemUnitsQuery
                         }).Result.StoreQuantity));

                List<string> serialsList = serialTransactionQuery.TableNoTracking.Where(a => (a.AddedInvoice == request.InvoiceType
                          || a.ExtractInvoice == request.InvoiceType) && a.IsDeleted == false).Select(a => a.SerialNumber).ToList();
                if (serialsList.Count > 0)
                {
                    //   var serialExist = serialsService.checkSerialBeforeSave(false, null, null, InvoiceTypeId, serialsList);
                    var serialsFromDb = serialTransactionQuery.TableNoTracking.Where(a => serialsList.Contains(a.SerialNumber)
               && a.AddedInvoice != null && a.ExtractInvoice == null && a.IsDeleted == false).ToList();
                    //&& a.AddedInvoice == InvoiceType
                    if (serialsFromDb.Count() != serialsList.Count())
                    {
                        CanDelete = false;
                    }
                    else
                    {
                        CanDelete = true;
                    }
                }
                else if (details.Count() > 0)
                {
                    CanDelete = false;
                }
                else
                {
                    CanDelete = true;
                }

            }

            return CanDelete;
        }
    }
}
