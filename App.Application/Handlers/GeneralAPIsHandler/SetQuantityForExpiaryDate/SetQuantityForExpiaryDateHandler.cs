using App.Application.Helpers.CalcItemQuantity;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Invoices;
using App.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.SetQuantityForExpiaryDate
{
    public class SetQuantityForExpiaryDateHandler : IRequestHandler<setQuantityForExpiaryDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;

        public SetQuantityForExpiaryDateHandler(IRepositoryQuery<InvGeneralSettings> generalSettings, IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery)
        {
            GeneralSettings = generalSettings;
            this.itemCardMasterRepository = itemCardMasterRepository;
            this.itemCardPartsQuery = itemCardPartsQuery;
            this.itemUnitsQuery = itemUnitsQuery;
            this.invoiceMasterQuery = invoiceMasterQuery;
            this.invoiceDetailsQuery = invoiceDetailsQuery;
        }

        public SetQuantityForExpiaryDateHandler(IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery)
        {
            this.invoiceDetailsQuery = invoiceDetailsQuery;
        }

        public async Task<ResponseResult> Handle(setQuantityForExpiaryDateRequest request, CancellationToken cancellationToken)
        {
            // get all dates don't expiared group by expiary date
            var expiaryOfInvoice = invoiceDetailsQuery.TableNoTracking
                            .Where(a => (request.invoiceId > 0 ? a.InvoiceId == request.invoiceId : true)
                                && a.ItemId == request.itemId //&& a.UnitId == request.unitId
                                && (request.EditedDate == null ? (a.ExpireDate > request.InvoiceDate) : (a.ExpireDate == request.EditedDate)))
                            .Select(a => new
                            {
                                expiaryOfInvoice = a.ExpireDate,
                                a.Quantity
                            }).OrderBy(a => a.expiaryOfInvoice).ToList().GroupBy(a => a.expiaryOfInvoice).ToList();
            double oldTotalQuantity = 0; // Total quantity of item entered by user in the same invoice
            var oldData1 = request.oldData.GroupBy(a => a.expiaryOfInvoice); // list of the entered expiary dates 
            oldTotalQuantity = request.oldData.Sum(a => a.QuantityOfDate);

            // calculate available quantity in store without expiared dates
            var TotalQuantity = CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
            {
                invoiceId = request.invoiceId,
                ItemId = request.itemId,
                UnitId = request.unitId,
                StoreId = request.storeId,
                invoiceTypeId = request.invoiceTypeId,
                invoiceDate = request.InvoiceDate,
                IsExpiared = false,
                ParentInvoiceType = "",
                ExpiryDate = null,
                GeneralSettings = GeneralSettings,
                invoiceDetailsQuery = invoiceDetailsQuery,
                invoiceMasterQuery = invoiceMasterQuery,
                itemCardMasterRepository  = itemCardMasterRepository,
                itemCardPartsQuery = itemCardPartsQuery,
                items =null,
                itemTypeId = 0,
                itemUnitsQuery= itemUnitsQuery
                
            }).Result.StoreQuantityWithOutInvoice;
            //CalcItemQuantityHandler.CalcItemQuantity(request.invoiceId, request.itemId, request.unitId, request.storeId, "", null, false, request.invoiceTypeId, request.InvoiceDate, null, 0).StoreQuantityWithOutInvoice;



            // check if 
            if (TotalQuantity < (request.quantity + oldTotalQuantity))
                return new ResponseResult { Data = null, Result = Result.QuantityNotavailable, Note = "QuantityNotavailable"
                , ErrorMessageAr = ErrorMessagesAr.QuantityNotAvailable , ErrorMessageEn=ErrorMessagesEn.QuantityNotAvailable};


            var expiaryList = new List<ExpiaryData>();
            if (expiaryOfInvoice.Count() > 0)
            {
                var qty = request.quantity;
                var setDecimal = GeneralSettings.TableNoTracking.Select(a => a.Other_Decimals).First();

                foreach (var expiary in expiaryOfInvoice)
                {
                    if (qty == 0) break;
                    var currentQuantity = CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
                    {
                        invoiceId = request.invoiceId,
                        ItemId = request.itemId,
                        UnitId = request.unitId,
                        StoreId = request.storeId,
                        invoiceTypeId = request.invoiceTypeId,
                        invoiceDate = request.InvoiceDate,
                        IsExpiared = true,
                        ParentInvoiceType = "",
                        ExpiryDate = expiary.Key,
                        GeneralSettings = GeneralSettings,
                        invoiceDetailsQuery = invoiceDetailsQuery,
                        invoiceMasterQuery = invoiceMasterQuery,
                        itemCardMasterRepository = itemCardMasterRepository,
                        itemCardPartsQuery = itemCardPartsQuery,
                        items = null,
                        itemTypeId = 0,
                        itemUnitsQuery= itemUnitsQuery
                    }).Result.StoreQuantityWithOutInvoice;


                    //CalcItemQuantity(request.invoiceId, request.itemId, request.unitId, request.storeId, "", expiary.Key, true, request.invoiceTypeId, request.InvoiceDate, null, 0).StoreQuantityWithOutInvoice;


                    if (oldData1.Count() > 0)
                    {

                        var currentQuantity1 = oldData1.Select(a => new { a.Key, x = currentQuantity - a.Sum(a => a.QuantityOfDate) })
                            .Where(a => a.Key == expiary.Key.Value.ToString("yyyy-MM-dd"));
                        currentQuantity = currentQuantity1.Select(a => a.x).FirstOrDefault(currentQuantity);
                    }
                    if (currentQuantity <= 0) continue;

                    var expiaryData = new ExpiaryData();
                    expiaryData.expiaryOfInvoice = expiary.Key.Value.ToString("yyyy-MM-dd");//.ToString("yyyy-MM-dd");
                    if (currentQuantity < qty)
                    {
                        expiaryData.QuantityOfDate = currentQuantity;
                    }
                    else
                    {
                        expiaryData.QuantityOfDate = Math.Round(qty, setDecimal);
                    }

                    qty = qty - expiaryData.QuantityOfDate;
                    expiaryData.discountValue = (expiaryData.QuantityOfDate / request.quantity) * request.discountValue;
                    expiaryData.totalPrice = (request.price * expiaryData.QuantityOfDate) - expiaryData.discountValue;
                    expiaryList.Add(expiaryData);

                }

            }
            var result = expiaryList.OrderBy(a => DateTime.Parse(a.expiaryOfInvoice)).ToList();
            return new ResponseResult { Data = expiaryList, DataCount = expiaryList.Count(), Result = expiaryList.Count() > 0 ? Result.Success : Result.NoDataFound };
        }
    }
}
