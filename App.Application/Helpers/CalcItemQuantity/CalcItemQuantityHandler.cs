using App.Domain.Entities.Setup;
using App.Domain.Models;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace App.Application.Helpers.CalcItemQuantity
{
    public static class CalcItemQuantityHandler
    {
        public static async Task<QuantityInStoreAndInvoice> CalcItemQuantity(CalcItemQuantityRequest request)
        {
            if (request.itemTypeId == 0)
            {
                request.itemTypeId = request.itemCardMasterRepository.TableNoTracking.Where(a => a.Id == request.ItemId).First().TypeId;
            }
            if (Lists.CompositeItemOnInvoice.Contains(request.invoiceTypeId.Value))
            {
                var composieItem = await CalcCompositeItemQuantity(new CalcCompositeItemQuantityRequest
                {
                    invoiceId = request.invoiceId,
                    ItemId = request.ItemId,
                    UnitId = request.UnitId,
                    StoreId = request.StoreId,
                    ParentInvoiceType = request.ParentInvoiceType,
                    ExpiryDate = null,
                    IsExpiared = false,
                    invoiceTypeId = request.invoiceTypeId,
                    invoiceDate = request.invoiceDate,
                    items = request.items,
                    itemTypeId = request.itemTypeId,
                    currentQuantity=request.currentQuantity,
                    itemCardMasterRepository = request.itemCardMasterRepository,
                    itemCardPartsQuery = request.itemCardPartsQuery,
                    itemUnitsQuery = request.itemUnitsQuery ,
                    GeneralSettings= request.GeneralSettings,
                    invoiceDetailsQuery=request.invoiceDetailsQuery,
                    invoiceMasterQuery=request.invoiceMasterQuery 
                });
                if (composieItem != null)
                    return composieItem;
            }

            return await CalcItemQuantityNotComposite(new CalcItemQuantityNotCompositeRequest
            {
                invoiceId = request.invoiceId,
                ItemId = request.ItemId,
                UnitId = request.UnitId,
                StoreId = request.StoreId,
                ParentInvoiceType = request.ParentInvoiceType,
                ExpiryDate = request.ExpiryDate,
                IsExpiared = request.IsExpiared,
                invoiceTypeId = request.invoiceTypeId,
                invoiceDate = request.invoiceDate,
                items = request.items,
                itemTypeId = request.itemTypeId,
                currentQuantity= request.currentQuantity,
                GeneralSettings = request.GeneralSettings,
                invoiceDetailsQuery = request.invoiceDetailsQuery,
                invoiceMasterQuery = request.invoiceMasterQuery,
                itemCardPartsQuery = request.itemCardPartsQuery,
                itemUnitsQuery = request.itemUnitsQuery
            });
        }
        public static async Task<QuantityInStoreAndInvoice> CalcCompositeItemQuantity(CalcCompositeItemQuantityRequest request)
        {
            var QuantityInStoreAndInvoice = new QuantityInStoreAndInvoice();

            var itemdata_ = request.itemCardMasterRepository.TableNoTracking.Include(a => a.Parts).Where(a => a.Id == request.ItemId);
            if (itemdata_.Count() > 0)
            {
                var selectedFactor = request.itemUnitsQuery.TableNoTracking.Where(a => a.ItemId == request.ItemId && a.UnitId == request.UnitId)
                              .Select(a => a.ConversionFactor).FirstOrDefault();

                var itemdata = itemdata_.First();
                if (itemdata.TypeId != (int)ItemTypes.Composite)
                    return null;
                double minimumStoreQuantityWithOutInvoice = 0.0;
                double minimumStoreQuantity = 0.0;
                double minimumInvoiceQuantity = 0.0;
                foreach (var item in itemdata.Parts)
                {
                    var qty = await CalcItemQuantityNotComposite(new CalcItemQuantityNotCompositeRequest
                    {
                        invoiceId = request.invoiceId,
                        ItemId = item.PartId,
                        UnitId = item.UnitId,
                        StoreId = request.StoreId,
                        ParentInvoiceType = request.ParentInvoiceType,
                        ExpiryDate = null,
                        IsExpiared = false,
                        invoiceTypeId = request.invoiceTypeId,
                        invoiceDate = request.invoiceDate ,
                        items = request.items,
                        itemTypeId = request.itemTypeId,
                        currentQuantity=request.currentQuantity,
                        GeneralSettings = request.GeneralSettings,
                        itemUnitsQuery = request.itemUnitsQuery,
                        itemCardPartsQuery = request.itemCardPartsQuery,
                        invoiceMasterQuery = request.invoiceMasterQuery,
                        invoiceDetailsQuery  = request.invoiceDetailsQuery
                    });
                   var roundDecimal  = request.GeneralSettings.TableNoTracking.First().Other_Decimals;

                    minimumStoreQuantityWithOutInvoice = minimumQtyOfCompositeItem(qty.StoreQuantityWithOutInvoice, item.Quantity, minimumStoreQuantityWithOutInvoice, roundDecimal);
                    minimumStoreQuantity = minimumQtyOfCompositeItem(qty.StoreQuantity, item.Quantity, minimumStoreQuantity, roundDecimal);
                    minimumInvoiceQuantity = minimumQtyOfCompositeItem(qty.InvoiceQuantity, item.Quantity, minimumInvoiceQuantity, roundDecimal);
                }
                int signal = GetSignal(request.invoiceTypeId.Value);
                double enteredQuantity = (request.items == null ? 0 : CalcQuantityFromRequest(request.items, request.ItemId, selectedFactor, signal, request.itemTypeId, request.itemCardPartsQuery,request.itemUnitsQuery,request.GeneralSettings,request.ExpiryDate,request.currentQuantity,request.invoiceDate,request.invoiceTypeId.Value));

                QuantityInStoreAndInvoice.StoreQuantityWithOutInvoice = minimumStoreQuantityWithOutInvoice + signal*enteredQuantity;
                QuantityInStoreAndInvoice.StoreQuantity = minimumStoreQuantity;
                QuantityInStoreAndInvoice.InvoiceQuantity = minimumInvoiceQuantity;
            }
            return QuantityInStoreAndInvoice;
        }
        public static async Task<QuantityInStoreAndInvoice> CalcItemQuantityNotComposite(CalcItemQuantityNotCompositeRequest request)
        {
            // invoiceId  دعشان اجيب الكميات بتاعت الصنف ف حاله التعديل ف استثني الفاتوره اللي بعدلها 
            var QuantityInStoreAndInvoice = new QuantityInStoreAndInvoice();
            var setingOfDecimal = request.GeneralSettings.TableNoTracking.First().Other_Decimals;
            // convert quantities to the selected unit in invoice
            var selectedFactor = request.itemUnitsQuery.TableNoTracking.Where(a => a.ItemId == request.ItemId && a.UnitId == request.UnitId)
                                .Select(a => a.ConversionFactor).FirstOrDefault();
            if (selectedFactor == 0) // if itemId or unitId doesn't exist
            {
                return QuantityInStoreAndInvoice;
            }

            // calc quantity in store
            // QUANTITY IN STOER + invoice
            var TotalQuantity = request.invoiceDetailsQuery.TableNoTracking
                                        .Include(a => a.InvoicesMaster)
                                        .Where(a => a.ItemId == request.ItemId && a.InvoicesMaster.StoreId == request.StoreId
                                              && (request.ExpiryDate != null ? a.ExpireDate == request.ExpiryDate : true) //calculate quantity of specific expiared date
                                              && (a.ItemTypeId == (int)ItemTypes.Expiary ? (!request.IsExpiared ? true : a.ExpireDate > request.invoiceDate.Date) : true)); //IsExpiared=false calculate total quantity of not expiared dates 
                                                                                                                                                            // quantity in small unit
            var TotalQuantityInSmallUnit = TotalQuantity.Sum(a => a.Quantity * a.Signal * a.ConversionFactor);
            // quantity with selected unit 
            var totalQty = TotalQuantityInSmallUnit / selectedFactor;
            QuantityInStoreAndInvoice.StoreQuantity = Math.Round(totalQty, setingOfDecimal);

            // calc quantity in invoice (main invoice , deleted invoice or retured invoice) so I use InvoiceType || ParentInvoiceCode instead of InvoiceId
            double invoiceQuantity = 0;
            // بحسب الكميه الموجوده فى الفاتوره عشان اخصمها من الكميه الموجوده فى المخزن لو الفاتوره كانت اضافه
            // وبزودها لو كانت الفاتوره صرف من المخزن السيجنال هي الفيصل 
            // بعتبر ان الكميه بتاعت الفاتوره مش موجوده لان وارد اليوزر يعدل عليها
            // الفرونت بيزودها عنده على ال StoreQuantityWithOutInvoice
            if (request.invoiceId > 0)
            {
                var mainInvoice = request.invoiceMasterQuery.TableNoTracking.Where(a => a.InvoiceId == request.invoiceId.Value).Select(a => new { a.InvoiceTypeId, a.InvoiceType }).ToList().First();
                TotalQuantity = TotalQuantity.Where(a => a.InvoicesMaster.InvoiceType == mainInvoice.InvoiceType || a.InvoicesMaster.ParentInvoiceCode == mainInvoice.InvoiceType);
                TotalQuantityInSmallUnit = TotalQuantity.Sum(a => a.Quantity * a.Signal * a.ConversionFactor);

                totalQty = TotalQuantityInSmallUnit / selectedFactor;
                invoiceQuantity = totalQty;
                QuantityInStoreAndInvoice.InvoiceQuantity = Math.Round(Math.Abs(totalQty), setingOfDecimal);

            }
            // الكمية فى المخزن 
            int signal = GetSignal(request.invoiceTypeId.Value);
            double enteredQuantity = (request.items == null ? 0 : CalcQuantityFromRequest(request.items, request.ItemId, selectedFactor, signal, request.itemTypeId, request.itemCardPartsQuery,request.itemUnitsQuery,request.GeneralSettings,request.ExpiryDate,request.currentQuantity,request.invoiceDate,request.invoiceTypeId.Value));
           if( Lists.returnInvoiceList.Contains(request.invoiceTypeId.Value))
            QuantityInStoreAndInvoice.StoreQuantityWithOutInvoice = Math.Round(QuantityInStoreAndInvoice.StoreQuantity
                                                                                +signal*enteredQuantity, setingOfDecimal);
           else
                QuantityInStoreAndInvoice.StoreQuantityWithOutInvoice = Math.Round(QuantityInStoreAndInvoice.StoreQuantity
                                                                             - invoiceQuantity + signal * enteredQuantity, setingOfDecimal);

            return QuantityInStoreAndInvoice;
        }
        private static double minimumQtyOfCompositeItem(double ComponentQtyInDB, double ComponentQtyInItem, double minimumQty,int RoundDecimal)
        {
            double availableQuantity =Math.Round( ComponentQtyInDB / ComponentQtyInItem ,RoundDecimal);


            if (minimumQty > availableQuantity || minimumQty == 0)
                minimumQty = availableQuantity;
            return minimumQty;
        }
        public static int GetSignal(int invoiceTypeId)
        {
            int signal = 1;
            var invoicesTypes = new List<int> { (int)Enums.DocumentType.DeletePurchase, (int)Enums.DocumentType.ReturnPurchase
                                            , (int)Enums.DocumentType.POS , (int)Enums.DocumentType.OutgoingTransfer ,(int)Enums.DocumentType.Sales ,  (int)Enums.DocumentType.DeleteAddPermission,(int)Enums.DocumentType.DeleteItemsFund
                                            ,  (int)Enums.DocumentType.ExtractPermission,(int)Enums.DocumentType.SafePayment,(int)Enums.DocumentType.BankPayment,(int)Enums.DocumentType.CompinedSafePayment,(int)Enums.DocumentType.CompinedBankPayment,(int)Enums.DocumentType.PermittedDiscount};

            if (invoicesTypes.Contains(invoiceTypeId))
                signal = -1;

            return signal;
        }
        public static double CalcQuantityFromRequest(List<CalcQuantityRequest> items, int currentItemId, double currentFactor, int signal, int currentItemTypeId, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery, IRepositoryQuery<InvGeneralSettings> GeneralSettings,DateTime? enteredExpiryDate,double currentQuantity , DateTime invoiceDate,int invoiceTypeId)
        {
            // get componnet of composite item
            if (currentItemTypeId != (int)ItemTypes.Composite) // فى حالة الصنف الحالى مش مركب -> هجيب مكونات الاصناف المركبه المبعوته فى باقي الفاتورة
            {
                var compositeItems = items.Where(a => a.itemTypeId == (int)ItemTypes.Composite);
                if (compositeItems.Count() > 0)
                {
                    List<CalcQuantityRequest> componnentItemInRequest = new List<CalcQuantityRequest>();
                    foreach (var item in compositeItems)
                    {
                        var componnentItems = setCompositItem(item.itemId, 0, item.enteredQuantity, itemCardPartsQuery,itemUnitsQuery);

                        componnentItems.ForEach(a =>
                                componnentItemInRequest.Add(new CalcQuantityRequest()
                                { itemId = a.ItemId, conversionFactor = a.ConversionFactor, enteredQuantity = a.Quantity, itemTypeId = (int)ItemTypes.Store }));
                    }
                    items.AddRange(componnentItemInRequest);
                    items.RemoveAll(a => a.itemTypeId == (int)ItemTypes.Composite); //loop بمسح كل الاصناف المركبه بعد اما افكها لمنع تكرار الفك فى ال 
                }
            }
            var AutoExtractExpireDate = GeneralSettings.TableNoTracking.First().Other_AutoExtractExpireDate; // (a.itemTypeId==(int)ItemTypes.Expiary ? a.enteredExpiryDate== enteredExpiryDate:true )
            var enteredQuantity = items.Where(a => a.itemId == currentItemId && 
                       (a.itemTypeId == (int)ItemTypes.Expiary  ?  
                                 (enteredExpiryDate != null ? a.enteredExpiryDate == enteredExpiryDate :a.enteredExpiryDate >= invoiceDate )
                                 : true) //(a.itemTypeId != (int)ItemTypes.Expiary || AutoExtractExpireDate)
            ).Sum(a => a.enteredQuantity * a.conversionFactor / currentFactor);// + ( currentQuantity);
            if (invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.AddPermission || invoiceTypeId == (int)DocumentType.itemsFund)
            {
                enteredQuantity += currentQuantity;
            }
            //enteredQuantity += currentQuantity;

            return enteredQuantity;
        }
        public static List<InvoiceDetailsRequest> setCompositItem(int itemId, int unitId, double qty, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery,IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            var componentItems = new List<InvoiceDetailsRequest>();
            var itemData = itemCardPartsQuery.TableNoTracking.Include(a => a.CardMaster).Include(a => a.Unit).Where(a => a.ItemId == itemId);
            var unitsOfComponnent = itemData.Select(e => e.PartId).ToList();
            var itemunit = itemUnitsQuery.TableNoTracking.Where(a => unitsOfComponnent.Contains(a.ItemId)).ToList();
            foreach (var item in itemData)
            {
                var componentItem = new InvoiceDetailsRequest();

                componentItem.ItemId = item.PartId;
                componentItem.UnitId = item.UnitId;
                componentItem.Quantity = qty * item.Quantity;
                //componentItem.ItemTypeId = item.CardMaster.TypeId;
                // componentItem.ItemCode = item.CardMaster.ItemCode;
                componentItem.parentItemId = itemId;
                componentItem.ConversionFactor = itemunit.Where(a => a.UnitId == item.UnitId).First().ConversionFactor;
                //  var itemDetails = ItemDetails(invoice, componentItem);

                componentItems.Add(componentItem);
            }
            return componentItems;
        }
        // اى حاجه 
    }
}
