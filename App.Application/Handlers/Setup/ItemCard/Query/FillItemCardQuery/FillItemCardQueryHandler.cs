using App.Application.Handlers.Helper.GetItemData;
using App.Application.Handlers.Helper.serialIsBinded;
using App.Application.Handlers.Invoices.PriceLists;
using App.Application.Helpers.CalcItemQuantity;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Domain.Models;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Infrastructure;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials.SerialsService;

namespace App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery
{
    public class FillItemCardQueryHandler : IRequestHandler<FillItemCardQueryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IBalanceBarcodeProcs balanceBarcode;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly iUserInformation iUserInformation;
        private readonly IRepositoryQuery<InvStoreBranch> SotreQuery;
        private readonly IMediator _mediator;
        private readonly IRepositoryQuery<InvPersonLastPrice> personLastPriceQuery;
         
        public FillItemCardQueryHandler(IRepositoryQuery<InvGeneralSettings> generalSettings, IBalanceBarcodeProcs balanceBarcode,
            IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery,
            IRepositoryQuery<InvSerialTransaction> serialTransactionQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery,
            IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery,
             iUserInformation iUserInformation, IRepositoryQuery<InvStoreBranch> SotreQuery, IRepositoryQuery<InvPersonLastPrice> personLastPriceQuery,
            IMediator mediator)
        {
            GeneralSettings = generalSettings;
            this.balanceBarcode = balanceBarcode;
            this.itemCardMasterRepository = itemCardMasterRepository;
            this.itemUnitsQuery = itemUnitsQuery;
            this.serialTransactionQuery = serialTransactionQuery;
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            this.invoiceMasterQuery = invoiceMasterQuery;
            this.itemCardPartsQuery = itemCardPartsQuery;
            _mediator = mediator;
            this.iUserInformation = iUserInformation;
            this.SotreQuery = SotreQuery;
            this.personLastPriceQuery = personLastPriceQuery;
        }
        public async Task<ResponseResult> Handle(FillItemCardQueryRequest request, CancellationToken cancellationToken)
        {
             if (request.invoiceId == null)
                request.invoiceId = 0;

            var userInfo = await iUserInformation.GetUserInformation();
            if (Lists.POSInvoicesList.Contains(request.InvoiceTypeId))
            {
                var storeID = SotreQuery.TableNoTracking.Where(h => userInfo.userStors.Contains(h.StoreId) && h.BranchId == userInfo.CurrentbranchId).Select(a => a.StoreId);//.Select(a => a.StoreBranches);

                request.storeId = storeID.FirstOrDefault();

            }
            bool IsAdd = false;
            if (Lists.InvoicesTypeOfAddingToStore.Contains(request.InvoiceTypeId))
                IsAdd = true;
            else if (Lists.InvoicesTypesOfExtractFromStore.Contains(request.InvoiceTypeId))
                IsAdd = false;


            var Settings_ = await GeneralSettings.SingleOrDefault(a => a.Id == 1);
            FillItemCardResponse item = new FillItemCardResponse();
            //hamada start
            //check if barcode balance
            //BalanceBarcodeProcs BalanceBarcode = new BalanceBarcodeProcs();
            //        BalanceBarcodeResult getitemBalanceCode = null;
            #region getItemData 
            ItemAllData ItemFullData = null;
            //start hamada
            BalanceBarcodeResult getitemBalanceCode = balanceBarcode.getItem(new BalanceBarcodeInput()
            {
                FullCode = request.ItemCode,
                ItemCodestart = Settings_.Barcode_ItemCodestart ? 1 : 0,

            });

            var serialRemovedInEdit_ = false;
            if (request.serialRemovedInEdit != null)
                serialRemovedInEdit_ = request.serialRemovedInEdit.Value;
            if (getitemBalanceCode != null && (request.InvoiceTypeId == (int)DocumentType.Sales || request.InvoiceTypeId == (int)DocumentType.POS))
            {
                ItemFullData = await GetItemData(request.UnitId, IsAdd, getitemBalanceCode.ItemCode.ToUpper(), serialRemovedInEdit_, request.invoiceType, request.storeId,request.InvoiceTypeId);
                if (ItemFullData.itemMaster.Count() != 0 || ItemFullData.itemData.Count() != 0)
                {
                    item.balanceBarcode = request.ItemCode;
                    request.ItemCode = getitemBalanceCode.ItemCode;
                    item.isBalanceBarcode = true;
                }

                else
                {
                    ItemFullData = await GetItemData(request.UnitId, IsAdd, request.ItemCode.ToUpper(), serialRemovedInEdit_, request.invoiceType,request.storeId, request.InvoiceTypeId); 
                    getitemBalanceCode = null;
                }
            }
            else
            {
                ItemFullData = ItemFullData = await GetItemData(request.UnitId, IsAdd, request.ItemCode.ToUpper(), serialRemovedInEdit_, request.invoiceType,request.storeId, request.InvoiceTypeId);
            }
             
            #endregion

            var itemMaster = ItemFullData.itemMaster;
            var ItemData = ItemFullData.itemData;

            //end hamada
            #region validationOfItem
            if (itemMaster.Count() == 0 && ItemData.Count() == 0)
                return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.NotExist, Note = Actions.NotFound, ErrorMessageAr = ErrorMessagesAr.ItemNotExist, ErrorMessageEn = ErrorMessagesEn.ItemNotExist };

            if (itemMaster.Count() > 0)
            {
                if (itemMaster.Select(a => a.Status).First() == (int)Status.Inactive)
                    return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.InActive, Note = Actions.ItemInactive, ErrorMessageAr = ErrorMessagesAr.ItemInActive, ErrorMessageEn = ErrorMessagesEn.ItemInActive };

                if (itemMaster.Select(a => a.TypeId).First() == (int)ItemTypes.Composite && !Lists.CompositeItemOnInvoice.Contains(request.InvoiceTypeId))
                    return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.CanNotAddCompositeItem, Note = Actions.CanNotAddCompositeItem 
                             , ErrorMessageAr=ErrorMessagesAr.CanNotAddCompositeItem ,ErrorMessageEn=ErrorMessagesEn.CanNotAddCompositeItem};

            }


            if (itemMaster.Count() > 0)
                if (itemMaster.Select(a => a.TypeId).First() == (int)ItemTypes.Note)
                {
                    item.ItemId = itemMaster.First().Id;
                    item.Item.ItemCode = itemMaster.First().ItemCode;
                    item.Item.ArabicName = itemMaster.First().ArabicName;
                    item.Item.LatinName = itemMaster.First().LatinName;
                    item.Item.TypeId = itemMaster.First().TypeId;
                    item.Vat = itemMaster.First().VAT;
                    item.ApplyVat = itemMaster.First().ApplyVAT;
                    //change when creating Discounts On Items
                    item.AutoDiscount = 0;
                    item.ImagePath = itemMaster.First().ImagePath; // for pos
                    return new ResponseResult() { Data = item, Id = item.ItemId, Result = Result.Success };

                }

            if (ItemData.Count() > 0 && (Lists.salesInvoicesList.Contains(request.InvoiceTypeId) || Lists.POSInvoicesList.Contains(request.InvoiceTypeId)))
                if (!ItemData.Select(a => a.Item.UsedInSales).First())
                    return new ResponseResult() { Id = item.ItemId, Result = Result.itemNotUsedInSales, ErrorMessageAr = ErrorMessagesAr.itemNotUsedInSales, ErrorMessageEn = ErrorMessagesEn.itemNotUsedInSales };

            List<string> serialsBinded = await _mediator.Send(new serialIsBindedRequest { invoiceType = "", itemCode = request.ItemCode, serialsRequest = null }); /*await serialIsBinded(request.ItemCode, null, "");*/
            if (serialsBinded.Count() > 0)
                return new ResponseResult() { Result = Result.bindedTransfer, ErrorMessageAr = ErrorMessagesAr.SerialsBinded, ErrorMessageEn = ErrorMessagesEn.SerialsBinded };

            #endregion

            #region getpriceListId
            int invoicesTypeFlag = 0;
            if (request.InvoiceTypeId == (int)DocumentType.Purchase || request.InvoiceTypeId == (int)DocumentType.wov_purchase
                || request.InvoiceTypeId == (int)DocumentType.PurchaseOrder)
                invoicesTypeFlag = 1;
            else if (request.InvoiceTypeId == (int)DocumentType.POS || request.InvoiceTypeId == (int)DocumentType.Sales ||
                     request.InvoiceTypeId == (int)DocumentType.OfferPrice)
                invoicesTypeFlag = 2;

            var LastPricesForItems = personLastPriceQuery.TableNoTracking.Where(a => a.invoiceTypeId == request.InvoiceTypeId && a.personId == request.PersonId);
            // get salePriceList According to settings
            var salePriceId = 0;
            if (invoicesTypeFlag == 2 && Settings_.other_ActivatePriceLists)
            {
                salePriceId = await _mediator.Send(new salePriceListsRequest()
                {
                    branchId = userInfo.CurrentbranchId,
                    personId = request.PersonId.Value,
                    salesManId = request.salesManId.Value,
                    employeeId = userInfo.employeeId,
                    PriceListId= request.PriceListId.Value,
                    invoiceTypeId =request.InvoiceTypeId,
                    setting = Settings_
                });
            }
            #endregion

            //the last in item card
            foreach (var itemcard in ItemData)
            {

                item.ItemId = itemcard.Item.Id;
                item.Item.ItemCode = itemcard.Item.ItemCode;
                item.Item.ArabicName = itemcard.Item.ArabicName;
                item.Item.LatinName = itemcard.Item.LatinName;
                item.UnitId = itemcard.UnitId;
                item.Unit.ArabicName = itemcard.Unit.ArabicName;
                item.Unit.LatinName = itemcard.Unit.LatinName;
                item.ConversionFactor = itemcard.ConversionFactor;
                item.Price = itemcard.PurchasePrice;
                // set price according to invoice type
                
                if (Lists.salesInvoicesList.Contains(request.InvoiceTypeId) || Lists.POSInvoicesList.Contains(request.InvoiceTypeId)
                     || Lists.ExtractPermissionInvoicesList.Contains(request.InvoiceTypeId) || 
                     request.InvoiceTypeId==(int)DocumentType.OfferPrice)
                    item.Price = itemcard.SalePrice1;

                //   اخر سعر شراء للمورد / اخر سعر بيع للعميل و قوائم الاسعار
                
                
                if ((  invoicesTypeFlag==1   && Settings_.Purchases_UseLastPrice) ||
                       (invoicesTypeFlag==2  && Settings_.other_ActivatePriceLists&& Settings_.PriceListType==(int)PriceListsType.LastPriceForCustomer))
                { 
                    var LastPrice = LastPricesForItems .Where(a=> a.itemId== itemcard.Item.Id &&
                                   (request.UnitId > 0 ? a.unitId == request.UnitId : a.unitId == itemcard.UnitId));

                    if (LastPrice.Count() > 0)
                        item.Price = LastPrice.First().price;
                }
                else if(invoicesTypeFlag==2 && Settings_.other_ActivatePriceLists)
                {
                    if (salePriceId == (int)SalePricesList.SalePrice1)
                        item.Price = itemcard.SalePrice1;
                    else if (salePriceId == (int)SalePricesList.SalePrice2)
                        item.Price = itemcard.SalePrice2;
                    else if (salePriceId == (int)SalePricesList.SalePrice3)
                        item.Price = itemcard.SalePrice3;
                    else if (salePriceId == (int)SalePricesList.SalePrice4)
                        item.Price = itemcard.SalePrice4;
                

                }



                item.Item.TypeId = itemcard.Item.TypeId;
                item.Vat = itemcard.Item.VAT;
                item.ApplyVat = itemcard.Item.ApplyVAT;
                //change when creating Discounts On Items
                item.AutoDiscount = 0;

                // if  return without invoice prevent return serial and expiary
                if(Lists.returnInvoiceList.Contains(request.InvoiceTypeId)
                    && string.IsNullOrEmpty(request.ParentInvoiceType))
                {
                    if (itemcard.Item.TypeId == (int)ItemTypes.Expiary)
                    {
                        return new ResponseResult()  { Id = item.ItemId,  Result = Result.canNotAddItem,
                            ErrorMessageAr = ErrorMessagesAr.canNotReturnExpiaryItem,
                            ErrorMessageEn = ErrorMessagesEn.canNotReturnExpiaryItem  };
                    }
                    if (itemcard.Item.TypeId == (int)ItemTypes.Serial)
                    {
                        return new ResponseResult() {  Id = item.ItemId, Result = Result.canNotAddItem,
                            ErrorMessageAr = ErrorMessagesAr.canNotReturnSerialItem,
                            ErrorMessageEn = ErrorMessagesEn.canNotReturnSerialItem };
                    }
                }

                    if (request.InvoiceTypeId == (int)DocumentType.ReturnPurchase || request.InvoiceTypeId == (int)DocumentType.Sales
                    || request.InvoiceTypeId == (int)DocumentType.POS || request.InvoiceTypeId == (int)DocumentType.ExtractPermission || request.InvoiceTypeId == (int)DocumentType.OutgoingTransfer || request.InvoiceTypeId == (int)DocumentType.IncomingTransfer)
                {
                    // check if serial exist
                    if (itemcard.Item.TypeId == (int)ItemTypes.Serial)
                    {
                       
                        var serialsOfInvoice = serialTransactionQuery.TableNoTracking
                             .Where(a =>
                                  // (request.invoiceId > 0 ? a.AddedInvoice == request.ParentInvoiceType : true)
                                  a.ItemId == item.ItemId &&
                               a.StoreId == request.storeId);// && a.IsDeleted == false);//.ToList();
                        bool isSerial = serialsOfInvoice.Select(a => a.SerialNumber).ToList().Contains(request.ItemCode.ToUpper());
                        var serialsDeleted = serialsOfInvoice.Where(a => a.IsDeleted).Select(a => a.SerialNumber);
                        item.ExtractedSerials = serialsOfInvoice.Where(a => a.ExtractInvoice != null && a.IsDeleted == false).Select(a => a.SerialNumber).ToList();
                        item.existedSerials = serialsOfInvoice.Where(a => a.ExtractInvoice == null && a.IsDeleted == false)
                            .Select(a => a.SerialNumber).ToList();

                        // السيريال طالما اتحذف من المشتريات ف اصبح مش موجود خالص 
                      
                        if (item.existedSerials.Contains(request.ItemCode.ToUpper()))// && !serialsDeleted.Contains(request.ItemCode.ToUpper())) 
                        {
                            item.listSerials.Add(request.ItemCode.ToUpper());
                            item.existedSerials.Remove(request.ItemCode.ToUpper());
                        }
                        else if (!serialRemovedInEdit_&& isSerial && (item.ExtractedSerials.Contains(request.ItemCode.ToUpper()) || serialsDeleted.Contains(request.ItemCode.ToUpper())))
                        {
                            var SerialBinded = serialsOfInvoice.Where(a => a.TransferStatus == TransferStatus.Binded && a.SerialNumber==request.ItemCode);
                            return new ResponseResult()
                            {
                                Data = null,
                                Id = item.ItemId,
                                Result = Result.NotExist,
                                Note = Actions.NotFound,
                                ErrorMessageAr = (SerialBinded.Count() > 0 ? ErrorMessagesAr.SerialsBinded : ErrorMessagesAr.ItemNotExist),
                                ErrorMessageEn = (SerialBinded.Count() > 0 ? ErrorMessagesEn.SerialsBinded : ErrorMessagesEn.ItemNotExist)
                            };

                        }
                        else
                        {
                            if(itemMaster.Count()==0)
                                return new ResponseResult()
                                {
                                    Data = null,
                                    Result = Result.NotExist,
                                    Note = Actions.NotFound,
                                    ErrorMessageAr =  ErrorMessagesAr.ItemNotExist,
                                    ErrorMessageEn =  ErrorMessagesEn.ItemNotExist
                                };

                        }

                    }

                    // in case of expiry item
                    if (itemcard.Item.TypeId == (int)ItemTypes.Expiary)
                    {
                        //if (request.InvoiceTypeId == (int)DocumentType.ReturnPurchase && string.IsNullOrEmpty(request.ParentInvoiceType))
                        //{
                        //    return new ResponseResult()
                        //    {
                        //        Id = item.ItemId,
                        //        Result = Result.canNotAddItem,
                        //        ErrorMessageAr = ErrorMessagesAr.canNotReturnExpiaryItem,
                        //        ErrorMessageEn = ErrorMessagesEn.canNotReturnExpiaryItem
                        //    };
                        //}


                        var expiaryOfInvoice = invoiceDetailsQuery.TableNoTracking
                             .Where(a => (//request.invoiceId > 0 ? a.InvoiceId == request.invoiceId : true &&  // عملته كومنت عشان كان مبيجبش الكميات ف التعديل لصنف الصلاحية
                             a.InvoicesMaster.StoreId == request.storeId)
                             && (request.InvoiceTypeId == (int)DocumentType.ExtractPermission ? true : a.ExpireDate > request.InvoiceDate.Date) && a.ItemId == item.ItemId)
                             .Select(a => new
                             {
                                 a.ItemId,
                                 a.ExpireDate,
                                 a.Items.Units.First(e => (request.UnitId > 0 ? e.UnitId == request.UnitId : true)).ConversionFactor,
                                 itemUnit = a.Items.Units
                             })
                            .OrderBy(a => a.ExpireDate).ToList().GroupBy(a => a.ExpireDate).ToList();

                        if (expiaryOfInvoice.Count() > 0)
                        {
                            double oldTotalQuantity = 0; // Total quantity of item entered by user in the same invoice

                            var oldData1 = request.oldData.GroupBy(a => a.expiaryOfInvoice); // list of the entered expiary dates.
                            bool isExpiray = true;
                            if (request.InvoiceTypeId == (int)DocumentType.ExtractPermission)
                                isExpiray = false;

                            foreach (var expiary in expiaryOfInvoice)
                            {
                                var selectedFactor = expiary.First().ConversionFactor;
                                double currentQuantity = CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
                                {
                                    invoiceId = request.invoiceId,
                                    ItemId = item.ItemId,
                                    UnitId = item.UnitId,
                                    StoreId = request.storeId,
                                    ParentInvoiceType = request.ParentInvoiceType,
                                    ExpiryDate = DateTime.Parse(expiary.Key.ToString()),
                                    IsExpiared = isExpiray,
                                    invoiceTypeId = request.InvoiceTypeId,
                                    invoiceDate = request.InvoiceDate,
                                    items = request.items,
                                    itemTypeId = item.Item.TypeId,
                                    GeneralSettings = GeneralSettings,
                                    invoiceDetailsQuery = invoiceDetailsQuery,
                                    invoiceMasterQuery = invoiceMasterQuery,
                                    itemCardMasterRepository = itemCardMasterRepository,
                                    itemCardPartsQuery = itemCardPartsQuery,
                                    itemUnitsQuery = itemUnitsQuery,
                                 
                                }).Result.StoreQuantityWithOutInvoice;

                             /*   var oldQuantity = oldData1.Select(a => new { a.Key, x = currentQuantity - a.Sum(e => e.QuantityOfDate * e.conversionFactor / selectedFactor) })
                                                  .Where(a => a.Key == expiary.Key.Value.ToString("yyyy-MM-dd"));

                                currentQuantity = oldQuantity.Select(a => a.x).FirstOrDefault(currentQuantity);*/

                                item.expiaryData.Add(new ExpiaryData()
                                {
                                    expiaryOfInvoice = Convert.ToDateTime(expiary.Key.ToString()).ToString("yyyy-MM-dd"),
                                    QuantityOfDate = Math.Round(currentQuantity, Settings_.Other_Decimals),
                                    price = expiary.First().itemUnit.Select(a =>
                                                 Lists.salesInvoicesList.Contains(request.InvoiceTypeId) ? a.SalePrice1 : a.PurchasePrice).First()
                                });
                            }
                            item.expiaryData.RemoveAll(a => a.QuantityOfDate == 0);

                        }
                    }

                }
            }
           if(item.ItemId==0)
                return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.NotExist, Note = Actions.NotFound, ErrorMessageAr = ErrorMessagesAr.ItemNotExist, ErrorMessageEn = ErrorMessagesEn.ItemNotExist };
            var NotExpiaryForAdd = !(request.InvoiceTypeId == (int)DocumentType.Purchase|| request.InvoiceTypeId == (int)DocumentType.wov_purchase || request.InvoiceTypeId == (int)DocumentType.AddPermission);
     
            if(!Lists.orderPurchaseAndOfferPrice.Contains(request.InvoiceTypeId) )
            {
                //if(string.IsNullOrEmpty(request.invoiceType))
                //{
                //    var k = 0;
                //}
                // لو مش مرتجع مبيعات بدون فاتورة
                //if (/*!(request.InvoiceTypeId == (int)DocumentType.ReturnSales &&*/ string.IsNullOrEmpty(request.invoiceType))

                    
                     var quantities = await CalcItemQuantityHandler.CalcItemQuantity(new CalcItemQuantityRequest
                         {
                            invoiceId = request.invoiceId,
                            ItemId = item.ItemId,
                            UnitId = item.UnitId,
                            StoreId = request.storeId,
                            ParentInvoiceType = request.ParentInvoiceType,
                            invoiceTypeId = request.InvoiceTypeId,
                            invoiceDate = request.InvoiceDate.Date,
                            items = request.items,
                            itemTypeId = item.Item.TypeId,
                            ExpiryDate = null,
                            IsExpiared = NotExpiaryForAdd,
                            GeneralSettings = GeneralSettings,
                            invoiceDetailsQuery = invoiceDetailsQuery,
                            invoiceMasterQuery = invoiceMasterQuery,
                            itemCardMasterRepository = itemCardMasterRepository,
                            itemCardPartsQuery = itemCardPartsQuery,
                            itemUnitsQuery = itemUnitsQuery
                         });
 
                        item.QuantityInStore = quantities.StoreQuantity;
                        item.StoreQuantityWithOutInvoice = quantities.StoreQuantityWithOutInvoice;

                
              
            }

            //if balanceBarcode
            if (getitemBalanceCode != null)
            {
                var BarcodeQTY = balanceBarcode.GetItemCost(item.Price, getitemBalanceCode.Qty, Settings_.barcodeType);
                item.itemQuantity = BarcodeQTY.QTY;
                item.itemCost = BarcodeQTY.ItemCost;

            }

            return new ResponseResult()
            {
                Data = ItemData.Any() ? item : null,
                Id = item.ItemId,
                Result = ItemData.Any() ? Result.Success : Result.NotExist
                ,
                ErrorMessageAr = ItemData.Any() ? "" : ErrorMessagesAr.ItemNotExist,
                ErrorMessageEn = ItemData.Any() ? "" : ErrorMessagesEn.ItemNotExist
            };
        }


        private async Task<ItemAllData> GetItemData(int? UnitId, bool IsAdd, string ItemCode,bool? serialRemovedInEdit, string invoiceType , int storeId , int invoiceTypeId)
        {
            var itemMaster = itemCardMasterRepository.TableNoTracking.Include(a => a.Serials)
                .Where(a =>  (a.ItemCode == ItemCode || a.NationalBarcode == ItemCode)
            || a.Serials.Where(e=>e.StoreId==storeId).Select(e => e.SerialNumber).Contains(ItemCode));// || a.Serials.Where(e=>e.ExtractInvoice==null).Select(e=>e.SerialNumber).Contains(request.ItemCode) ).ToList();

            if (serialRemovedInEdit == null)
                serialRemovedInEdit = false;

            var itemId = 0;
            if (serialRemovedInEdit.Value)
            {
                var itemexistInSerial = serialTransactionQuery.TableNoTracking.Where(a => a.SerialNumber == ItemCode && a.ExtractInvoice == invoiceType);//.First().ItemId;
                if (itemexistInSerial.Any())
                    itemId = itemexistInSerial.First().ItemId;
            }
            //    var itemFromSerial = serialTransactionQuery.TableNoTracking.Where(a => a.SerialNumber == request.ItemCode)
            //      .Select(a => a.ItemId).ToList().First();
            var ItemData = itemUnitsQuery.TableNoTracking
             .Include(a => a.Item).ThenInclude(a => a.Serials)
             .Include(a => a.Unit)
             .Where(a => ((a.Item.ItemCode == ItemCode
                                          || a.Barcode == ItemCode
                                          || a.Item.NationalBarcode == ItemCode
                                           || (serialRemovedInEdit.Value && itemId > 0 ? a.ItemId == itemId : a.Item.Serials.Where(s => s.ExtractInvoice == null || s.TransferStatus == TransferStatus.Binded).Select(e => e.SerialNumber).Contains(ItemCode))))
                                             && a.Item.Status == (int)Status.Active ).ToList();// && // بجيب السيريال متاح ف انهي صنف
            if (UnitId > 0)
                ItemData = ItemData.Where(a => a.UnitId == UnitId).ToList();
            else
            {
                var ItemData_ = ItemData.Where(a => a.Barcode == ItemCode).ToList();
                if (ItemData_.Count() == 0)
                    ItemData = ItemData.Where(a => IsAdd == true ? (a.Item.DepositeUnit == a.UnitId) : (a.Item.WithdrawUnit == a.UnitId)).ToList();
                else
                    ItemData = ItemData_;

            }

            return new ItemAllData()
            {
                itemData = ItemData,
                itemMaster = itemMaster
            };

        }
    }

}
