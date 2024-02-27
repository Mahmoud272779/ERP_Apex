using App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod;
using App.Application.Handlers.Invoices.OfferPrice.TransferToSales;
using App.Application.Handlers.InvoicesHelper.calcQyt;
using App.Application.Handlers.InvoicesHelper.MergeItems;
using App.Application.Helpers.POSHelper;
using App.Application.Services.HelperService.EmailServices;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Setup;
using App.Domain.Models;
using App.Domain.Models.Common;
using App.Domain.Models.Request.General;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Request.Store.Invoices;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Infrastructure;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System.IO;
using System.Text.RegularExpressions;
using static App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials.SerialsService;
using static App.Application.Services.Reports.Items_Prices.Rpt_Store;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Services.Process.Invoices.General_APIs
{
    public class GeneralAPIsService : IGeneralAPIsService
    {
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;

        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryCommand<InvStpItemCardUnit> itemUnitsCommand;
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;


        private readonly IRepositoryQuery<InvoiceSerialize> invoiceSerializeQuery;
        private readonly IRepositoryQuery<SystemHistoryLogs> _systemHistoryLogsQuery;
        private readonly IRepositoryQuery<GlReciepts> _gLRecieptQuery;
        private readonly IEmailService _emailService;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryCommand<InvoiceSerialize> invoiceSerializeCommand;

        private readonly IEditedItemsService editedItemsService;
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvStpStores> storeQuery;
        private readonly ICalculationSystemService CalculationSystemService;
        private readonly iUserInformation Userinformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IBalanceBarcodeProcs balanceBarcode;
        private readonly IReportFileService _iReportFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepositoryCommand<ReportFiles> _reportFileCommand;    
        private readonly IRepositoryCommand<ReportManger> _reportManagerCommand;
        private readonly IRepositoryQuery<InvStoreBranch> invStoreBranchQuery;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery;
        private readonly IMediator _mediator;

        // private readonly IRepositoryCommand<ReportFiles> _reportCommand;
        public GeneralAPIsService(IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery,
                                 IRepositoryQuery<InvStpItemCardUnit> _itemUnitsQuery,
                                 IRepositoryQuery<InvSerialTransaction> _serialTransactionQuery,
                                 IRepositoryQuery<InvStpItemCardMaster> ItemCardMasterRepository,
                                 IRepositoryQuery<InvGeneralSettings> generalSettings,
                                 IRepositoryQuery<InvoiceSerialize> invoiceSerializeQuery,
                                 IRepositoryQuery<SystemHistoryLogs> SystemHistoryLogsQuery,
                                 IRepositoryQuery<GlReciepts> GLRecieptQuery,
                                 IEmailService emailService,
                                 IRepositoryQuery<InvStpUnits> InvStpUnitsQuery,
                                 iUserInformation iUserInformation,
                                 IRepositoryQuery<userAccount> userAccountQuery,
                                 IRepositoryCommand<InvStpItemCardUnit> ItemUnitsCommand,
                                 IRepositoryCommand<InvoiceSerialize> invoiceSerializeCommand,
                                 ICalculationSystemService CalculationSystemService, iUserInformation Userinformation,
                                 IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IEditedItemsService editedItemsService,
                                 IRepositoryQuery<InvPersons> PersonRepositorQuery, IRepositoryQuery<InvStpStores> storeQuery,
                                 IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery, IRoundNumbers roundNumbers, IBalanceBarcodeProcs BalanceBarcode,
                                 IReportFileService iReportFileService, IWebHostEnvironment webHostEnvironment,
                                 IRepositoryCommand<ReportFiles> reportFileCommand,
                                 IRepositoryCommand<ReportManger> reportManagerCommand, IRepositoryQuery<InvStoreBranch> invStoreBranchQuery
            , IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery, IMediator mediator)
        {
            invoiceDetailsQuery = _InvoiceDetailsQuery;
            itemUnitsQuery = _itemUnitsQuery;
            serialTransactionQuery = _serialTransactionQuery;
            itemCardMasterRepository = ItemCardMasterRepository;
            itemUnitsCommand = ItemUnitsCommand;
            GeneralSettings = generalSettings;
            this.invoiceSerializeQuery = invoiceSerializeQuery;
            _systemHistoryLogsQuery = SystemHistoryLogsQuery;
            _gLRecieptQuery = GLRecieptQuery;
            _emailService = emailService;
            _invStpUnitsQuery = InvStpUnitsQuery;
            _iUserInformation = iUserInformation;
            _userAccountQuery = userAccountQuery;
            this.invoiceSerializeCommand = invoiceSerializeCommand;
            this.CalculationSystemService = CalculationSystemService;
            this.invoiceMasterQuery = invoiceMasterQuery;
            this.editedItemsService = editedItemsService;
            this.PersonRepositorQuery = PersonRepositorQuery;
            this.InvoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            this.Userinformation = Userinformation;
            this.storeQuery = storeQuery;
            this.roundNumbers = roundNumbers;
            balanceBarcode = BalanceBarcode;
            _iReportFileService = iReportFileService;
            _webHostEnvironment = webHostEnvironment;
            _reportFileCommand = reportFileCommand;
            _reportManagerCommand = reportManagerCommand;
            this.invStoreBranchQuery = invStoreBranchQuery;
            this.itemCardPartsQuery = itemCardPartsQuery;
            _mediator = mediator;
        }

        public async Task<QuantityInStoreAndInvoice> CalculateItemQuantity(int ItemId, int UnitId, int StoreId, string ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared, int invoiceId, DateTime invoiceDate, int? invoiceTypeId, List<CalcQuantityRequest> items)
        {
            return CalcItemQuantity(invoiceId, ItemId, UnitId, StoreId, ParentInvoiceType, ExpiryDate, false, invoiceTypeId, invoiceDate,items,0);
        }


        public async Task<serialsReponse> CheckSerials(serialsRequest request)
        {
            return CheckSerial(request, false);
        }

        public async Task<ResponseResult> GetItemsDropDown(DropDownRequest request)
        {
   
            var ItemsList = itemCardMasterRepository.TableNoTracking.Include(a=>a.Units)
                                                    .Where(e=> request.invoiceTypeId != null ? (Lists.salesInvoicesList.Contains(request.invoiceTypeId.Value) ? e.UsedInSales : true) : true )
                                                    .Where(e=> request.isSearchByCode != true ? e.Status == (int)Status.Active:true)
                                                    .Where(e=> request.invoiceTypeId !=null ? (Lists.CompositeItemOnInvoice.Contains(request.invoiceTypeId.Value) ? true : e.TypeId != (int)ItemTypes.Composite) : true)
                                                    .Where(e=> request.isSearchByCode == true ? (e.ItemCode == request.SearchCriteria || e.Units.Where(e => e.Barcode == request.SearchCriteria).Any() || e.NationalBarcode == request.SearchCriteria) : true)
                                                    .Where(e=> request.SearchCriteria != null  && request.isSearchByCode == false? (e.ArabicName.Contains(request.SearchCriteria) || e.LatinName.Contains(request.SearchCriteria)) : true)
                                                    .Where(x => request.itemType != 0 ? (x.TypeId == request.itemType) : true)
                                                    .Select(a => new { a.Id, a.ItemCode, a.ArabicName, a.LatinName, itemType = a.TypeId, a.Status })
                                                    //.OrderBy(a => a.ArabicName)
                                                    .ToList();

            if(request.invoiceTypeId!=null)
                if (Lists.transferStore.Contains(request.invoiceTypeId.Value))
                {
                    ItemsList = ItemsList.Where(a => a.itemType != (int)ItemTypes.Composite && a.itemType != (int)ItemTypes.Service).ToList();
                }

            double MaxPageNumber = ItemsList.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                ItemsList = ItemsList.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }

            var totalCount = itemCardMasterRepository.TableNoTracking.Where(e => e.Status == (int)Status.Active).Count();
            if (request.isSearchByCode != true)
            {
                ItemsList = ItemsList.Where(x => x.Status == 1).ToList();
            }
            return new ResponseResult()
            {
                Data = ItemsList,
                DataCount = ItemsList.Count(),
                Id = null,
                Result = ItemsList.Any() ? Result.Success : Result.Failed,
                TotalCount = totalCount,
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : ""),

            };


            //return new ResponseResult() { Data = ItemsList, Id = null, Result = ItemsList.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetItemUnitsDropDown(int itemId ,string? barcode)
        {
            var UnitsList = itemUnitsQuery.TableNoTracking.Where(e => e.ItemId == itemId )
                .Include(a => a.Unit).Select(a => new { a.UnitId, a.Unit.Code, a.Unit.ArabicName, a.Unit.LatinName ,a.Barcode})
                .OrderByDescending(a=> (string.IsNullOrEmpty(barcode)?1==1: a.Barcode==barcode.ToUpper())).ThenBy(a=>a.UnitId);

            return new ResponseResult() { Data = UnitsList, Id = null, Result = UnitsList.Any() ? Result.Success : Result.Failed };


        }
        public async Task<ResponseResult> UpdateCanDeleteInItemUnits(int itemId, int? UnitId, bool delete)
        {
            var updatedItem = itemUnitsQuery.FindAll(a => a.ItemId == itemId && (UnitId != null ? a.UnitId == UnitId : 1 == 1));

            if (updatedItem.Count > 0)
            {
                foreach (var item in updatedItem)
                {
                    item.WillDelete = delete;
                    await itemUnitsCommand.UpdateAsyn(updatedItem);
                }
            }
            return new ResponseResult() { Data = null, Id = null, Result = updatedItem == null ? Result.Failed : Result.Success };

        }
        public async Task<ResponseResult> GetItemsInPartsDropDown()
        {
            var ItemsList = itemCardMasterRepository.TableNoTracking.Where(e => e.Status == (int)Status.Active && e.TypeId == (int)ItemTypes.Store).Select(a => new { a.Id, a.ItemCode, a.ArabicName, a.LatinName });

            return new ResponseResult() { Data = ItemsList, Id = null, Result = ItemsList.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> mergeTotalItems(List<InvoiceDetailsRequest> list, int invoiceType)
        {
            var listResult = await MergeItems(list, invoiceType);
            return new ResponseResult() { Data = listResult, Id = null, Result = Result.Success };

        }
        public async Task<List<InvoiceDetailsRequest>> MergeItems(List<InvoiceDetailsRequest> list, int invoiceType)
        {
            var listDto = new List<InvoiceDetailsRequest>(); // list which will return after marge

            //    var SettingsOfOtherDecimal = (forSave?RoundNumbers.defaultDecimal:  GeneralSettings.TableNoTracking.First().Other_Decimals);
            foreach (var item in list)
            {
                var margeItemDto = new InvoiceDetailsRequest();
                if (listDto.Count() > 0)
                {
                    if (item.ItemTypeId == (int)ItemTypes.Serial)
                    {
                        MergeItemsMethod.AddMergeItem(listDto, item, margeItemDto);
                        continue;
                    }

                    var x = listDto.FirstOrDefault(q => q.ItemId == item.ItemId && q.UnitId == item.UnitId
                            && q.Price == item.Price && q.ItemTypeId==item.ItemTypeId);

                    if (x != null)
                    {
                        if (item.ItemTypeId == (int)ItemTypes.Expiary && item.ExpireDate != x.ExpireDate)
                        {
                            MergeItemsMethod.AddMergeItem(listDto, item, margeItemDto);
                        }
                        else
                        {
                            x.ExpireDate = item.ExpireDate;
                            x.ItemTypeId = item.ItemTypeId;
                            x.SplitedDiscountValue += item.SplitedDiscountValue;
                            x.SplitedDiscountRatio = item.SplitedDiscountRatio;
                            x.Quantity += item.Quantity;
                            x.Price = item.Price;
                            x.VatRatio = item.VatRatio;
                            x.VatValue += item.VatValue;
                            //  x.DiscountRatio += item.DiscountRatio;
                            x.DiscountValue += item.DiscountValue;
                            x.ConversionFactor = item.ConversionFactor;
                            x.Total += item.Total;
                            x.UnitId = item.UnitId;
                            x.SizeId = item.SizeId;
                            x.TotalWithSplitedDiscount += item.TotalWithSplitedDiscount;
                            x.DiscountRatio = (x.Total + x.DiscountValue) == 0 ? 0 : (x.DiscountValue / (x.Total + x.DiscountValue)) * 100;// Math.Round( (x.DiscountValue / (x.Total+x.DiscountValue)) * 100 , SettingsOfOtherDecimal);
                            x.isDiscountRatioItem = item.isDiscountRatioItem;
                            x.VatValueForPrint = item.VatValueForPrint;
                            x.Quantity = Math.Round(x.Quantity, (int)generalEnum.vatDecimal);
                        }
                        
                    }
                    else
                    {
                        MergeItemsMethod.AddMergeItem(listDto, item, margeItemDto);
                    }
                }
                else
                {
                    MergeItemsMethod.AddMergeItem(listDto, item, margeItemDto);
                }

            }
            return listDto;
        }
    
        public bool checkitemsFromOutgoingTransfer(List<InvoiceDetailsRequest> invoiceDetails, ICollection<InvoiceDetails> outgoingDetails)
        {

            if (outgoingDetails.Count() != invoiceDetails.Count)
            {
                return false;
            }
            var outgoing = outgoingDetails.Select(h => new { h.ItemId, IndexOfItem = h.indexOfItem }).ToList();
            var frontitem = invoiceDetails.Select(a => new { a.ItemId, a.IndexOfItem }).ToList();
            var res = outgoing.Except(frontitem);
            var res1 = frontitem.Except(outgoing);
            //var res = outgoing.Intersect(frontitem);
            return (res.Count() <= 0 && res1.Count() <= 0);//.Count()==outgoing.Count();
        }
        // get all required data of item 
        /*public async Task<ResponseResult> FillItemCardQuery(FillItemCardRequest request)
        {

            if (request.invoiceId == null)
                request.invoiceId = 0;

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
            ItemAllData ItemFullData = null;

            BalanceBarcodeResult getitemBalanceCode = balanceBarcode.getItem(new BalanceBarcodeInput()
            {
                FullCode = request.ItemCode,
                ItemCodestart = Settings_.Barcode_ItemCodestart ? 1 : 0,

            });

            if (getitemBalanceCode != null && (request.InvoiceTypeId == (int)DocumentType.Sales || request.InvoiceTypeId == (int)DocumentType.POS))
            {
                ItemFullData = GetItemData(request.UnitId, IsAdd, getitemBalanceCode.ItemCode);
                if (ItemFullData.itemMaster.Count() != 0 || ItemFullData.itemData.Count() != 0)
                {
                    item.balanceBarcode = request.ItemCode;
                    request.ItemCode = getitemBalanceCode.ItemCode;
                    item.isBalanceBarcode = true;
                }

                else
                {
                    ItemFullData = GetItemData(request.UnitId, IsAdd, request.ItemCode);
                    getitemBalanceCode = null;
                }
            }
            else
            {
                ItemFullData = GetItemData(request.UnitId, IsAdd, request.ItemCode);

            }




            var itemMaster = ItemFullData.itemMaster;
            var ItemData = ItemFullData.itemData;

            //end hamada

            if (itemMaster.Count() == 0 && ItemData.Count() == 0)
                return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.NotExist, Note = Actions.NotFound, ErrorMessageAr = ErrorMessagesAr.ItemNotExist, ErrorMessageEn = ErrorMessagesEn.ItemNotExist };

             if (itemMaster.Count() > 0)
            {
                if (itemMaster.Select(a => a.Status).First() == (int)Status.Inactive)
                    return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.InActive, Note = Actions.ItemInactive, ErrorMessageAr = ErrorMessagesAr.ItemInActive, ErrorMessageEn = ErrorMessagesEn.ItemInActive };

                if (itemMaster.Select(a => a.TypeId).First() == (int)ItemTypes.Composite && !Lists.CompositeItemOnInvoice.Contains(request.InvoiceTypeId))
                    return new ResponseResult() { Data = null, Id = item.ItemId, Result = Result.CanNotAddCompositeItem, Note = Actions.CanNotAddCompositeItem };

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
                    //chande when creating Discounts On Items
                    item.AutoDiscount = 0;
                    return new ResponseResult() { Data = item, Id = item.ItemId, Result = Result.Success };

                }

            if (ItemData.Count() > 0 && (Lists.salesInvoicesList.Contains(request.InvoiceTypeId) || Lists.POSInvoicesList.Contains(request.InvoiceTypeId)))
                if (!ItemData.Select(a => a.Item.UsedInSales).First())
                    return new ResponseResult() { Id = item.ItemId, Result = Result.itemNotUsedInSales, ErrorMessageAr = ErrorMessagesAr.itemNotUsedInSales, ErrorMessageEn = ErrorMessagesEn.itemNotUsedInSales };

            List<string> serialsBinded = await serialIsBinded(request.ItemCode, null, "");
            if (serialsBinded.Count() > 0)
                return new ResponseResult() { Result = Result.bindedTransfer, ErrorMessageAr = ErrorMessagesAr.SerialsBinded, ErrorMessageEn = ErrorMessagesEn.SerialsBinded };
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
                //if (Lists.purchasesInvoicesList.Contains(request.InvoiceTypeId))
                //    item.Price = itemcard.PurchasePrice;
                if (Lists.salesInvoicesList.Contains(request.InvoiceTypeId) || Lists.POSInvoicesList.Contains(request.InvoiceTypeId)
                     || Lists.ExtractPermissionInvoicesList.Contains(request.InvoiceTypeId))
                    item.Price = itemcard.SalePrice1;

                //    اخر سعر شراء للمورد / اخر سعر بيع للعميل
                if ((request.InvoiceTypeId == (int)DocumentType.Purchase && Settings_.Purchases_UseLastPrice) ||
                     ((request.InvoiceTypeId == (int)DocumentType.Sales||
                             request.InvoiceTypeId == (int)DocumentType.POS) && Settings_.LastPriceForCustomer) )
                {

                    var LastPrice = invoiceDetailsQuery.TableNoTracking.Where(a => a.InvoicesMaster.InvoiceTypeId == request.InvoiceTypeId
                   && a.InvoicesMaster.PersonId == request.PersonId && a.ItemId == itemcard.Item.Id
                   && (request.UnitId > 0 ? a.UnitId == request.UnitId : a.UnitId == itemcard.UnitId)).OrderBy(a => a.Id).ToList();//.OrderByDescending(a => a.Id).ToList();

                    if (LastPrice.Count() > 0)
                        item.Price = LastPrice.Last().Price;
                }



                item.Item.TypeId = itemcard.Item.TypeId;
                item.Vat = itemcard.Item.VAT;
                item.ApplyVat = itemcard.Item.ApplyVAT;
                //change when creating Discounts On Items
                item.AutoDiscount = 0;

                // if sales or returns
                if (request.InvoiceTypeId == (int)DocumentType.ReturnPurchase || request.InvoiceTypeId == (int)DocumentType.Sales
                    || request.InvoiceTypeId == (int)DocumentType.POS || request.InvoiceTypeId == (int)DocumentType.ExtractPermission || request.InvoiceTypeId == (int)DocumentType.OutgoingTransfer || request.InvoiceTypeId == (int)DocumentType.IncomingTransfer)
                {
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
                        if (item.existedSerials.Contains(request.ItemCode.ToUpper()))
                        {
                            item.listSerials.Add(request.ItemCode.ToUpper());
                            item.existedSerials.Remove(request.ItemCode.ToUpper());
                        }
                        else if (isSerial && (item.ExtractedSerials.Contains(request.ItemCode.ToUpper()) || serialsDeleted.Contains(request.ItemCode.ToUpper())))
                        {
                            var SerialBinded = serialsOfInvoice.Where(a => a.TransferStatus == TransferStatus.Binded);
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


                    }

                    // in case of expiry item
                    if (itemcard.Item.TypeId == (int)ItemTypes.Expiary)
                    {


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
                                double currentQuantity = CalcItemQuantity(request.invoiceId, item.ItemId, item.UnitId, request.storeId, request.ParentInvoiceType,
                                    DateTime.Parse(expiary.Key.ToString()), isExpiray, request.InvoiceTypeId, request.InvoiceDate , request.items,item.Item.TypeId ).StoreQuantityWithOutInvoice;
                                var oldQuantity = oldData1.Select(a => new { a.Key, x = currentQuantity - a.Sum(e => e.QuantityOfDate * e.conversionFactor / selectedFactor) })
                                                  .Where(a => a.Key == expiary.Key.Value.ToString("yyyy-MM-dd"));

                                currentQuantity = oldQuantity.Select(a => a.x).FirstOrDefault(currentQuantity);

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
            // calculate quantity of selected item in store
           // item.QuantityAvailable = await CheckQuantityInSettings(request.InvoiceTypeId);

            var quantities = CalcItemQuantity(request.invoiceId, item.ItemId, item.UnitId, request.storeId
                       , request.ParentInvoiceType, null, true, request.InvoiceTypeId, request.InvoiceDate.Date, request.items, item.Item.TypeId);
            item.QuantityInStore = quantities.StoreQuantity;
            item.StoreQuantityWithOutInvoice = quantities.StoreQuantityWithOutInvoice;

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

            //   return item;
        }
        */
        public async Task<List<string>> serialIsBinded(string itemCode, List<string>? serialsRequest, string invoiceType)
        {
          //  var serialsItems = details.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).Select(a =>new { a.ListSerials, a.IndexOfItem});

            // get binded serials 
            var serialsBinded = serialTransactionQuery.TableNoTracking
                             .Where(a => a.TransferStatus == TransferStatus.Binded &&
                             a.ExtractInvoice != invoiceType &&
                             (serialsRequest != null ? serialsRequest.Contains(a.SerialNumber) : a.SerialNumber == itemCode)).Select(a => a.SerialNumber).ToList();

            return serialsBinded;
        }
        private ItemAllData GetItemData(int? UnitId, bool IsAdd, string ItemCode)
        {
            var itemMaster = itemCardMasterRepository.TableNoTracking.Include(a => a.Serials).Where(a => (a.ItemCode == ItemCode
                                            || a.NationalBarcode == ItemCode) || a.Serials.Select(e => e.SerialNumber).Contains(ItemCode));// || a.Serials.Where(e=>e.ExtractInvoice==null).Select(e=>e.SerialNumber).Contains(request.ItemCode) ).ToList();


            //    var itemFromSerial = serialTransactionQuery.TableNoTracking.Where(a => a.SerialNumber == request.ItemCode)
            //      .Select(a => a.ItemId).ToList().First();
            var ItemData = itemUnitsQuery.TableNoTracking
             .Include(a => a.Item)
             .Include(a => a.Unit)
             .Where(a => ((a.Item.ItemCode == ItemCode
                                          || a.Barcode == ItemCode || a.Item.NationalBarcode == ItemCode
                                           || a.Item.Serials.Where(s => s.ExtractInvoice == null || s.TransferStatus == TransferStatus.Binded).Select(e => e.SerialNumber).Contains(ItemCode)) && // بجيب السيريال متاح ف انهي صنف
                                          (UnitId > 0 ? a.UnitId == UnitId :
                                                    (a.Barcode == ItemCode ? (1 == 1) : (IsAdd == true ? a.Item.DepositeUnit == a.UnitId : a.Item.WithdrawUnit == a.UnitId)))
                                          && a.Item.Status == (int)Status.Active)).ToList();

            return new ItemAllData()
            {
                itemData = ItemData,
                itemMaster = itemMaster
            };

        }

     
        //public int Autocode(int BranchId, int invoiceType)
        //{
        //    var Code = 1;
        //    Code = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

        //    if (Code != null)
        //        Code++;

        //    return Code;
        //}


        public async Task<ResponseResult> NavigationStep(int invoiceTypeId, int invoiceCode, bool NextCode)
        {

            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            bool showOthoerInvoice = POSHelper.showOthorInv(invoiceTypeId, userInfo);
            int invoiceTypeDel = (invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            int LastCode = InvoiceMasterRepositoryQuery.GetMaxCode(a => a.InvoiceId, q =>
                q.BranchId == userInfo.CurrentbranchId
                && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && (NextCode ? q.InvoiceId > invoiceCode : q.InvoiceId < invoiceCode)
                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));
            return new ResponseResult() { Data = LastCode };
        }

        //private bool showOthorInv(int invoiceTypeId, UserInformationModel userInfo)
        //{
        //    if (invoiceTypeId == (int)DocumentType.POS)
        //        return userInfo.otherSettings.posShowOtherPersonsInv;
        //    if (invoiceTypeId == (int)DocumentType.Sales)
        //        return userInfo.otherSettings.salesShowOtherPersonsInv;

        //    if (invoiceTypeId == (int)DocumentType.Purchase)
        //        return userInfo.otherSettings.purchasesShowOtherPersonsInv;

        //    return false;
        //}
        public async Task<ResponseResult> setPOSstartup(int invoiceTypeId)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            bool showOthoerInvoice = POSHelper.showOthorInv(invoiceTypeId, userInfo);
            int invoiceTypeDel = (invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            int LastCode = InvoiceMasterRepositoryQuery.GetMaxCode(a => a.InvoiceId, q =>
                q.BranchId == userInfo.CurrentbranchId && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));


            var nextCode = POSHelper.Autocode(userInfo.CurrentbranchId, invoiceTypeId,InvoiceMasterRepositoryQuery);

            var defaultCustomer = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == 2)
                .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName }).ToList().FirstOrDefault();
            var defaultStore = storeQuery.TableNoTracking.Where(a => a.StoreBranches.First().BranchId == userInfo.CurrentbranchId)
                .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName }).ToList().FirstOrDefault();

            //GeneralSetting
            var generalSettings = GeneralSettings.TableNoTracking.FirstOrDefault();



            var res = new PosDto()
            {
                LastCode = LastCode,
                nextCode = nextCode,
                customerId = defaultCustomer.Id,
                customerCode = defaultCustomer.Code,
                customerAr = defaultCustomer.ArabicName,
                customerEn = defaultCustomer.LatinName,
                POSgeneralSettingsint = new Domain.Models.Response.Store.Invoices.GeneralSettings()
                {
                    Pos_ActiveCashierCustody = generalSettings.Pos_ActiveCashierCustody,
                    Pos_ActiveDiscount = generalSettings.Pos_ActiveDiscount,
                    Pos_ActivePricesList = generalSettings.Pos_ActivePricesList,
                    Pos_DeferredSale = generalSettings.Pos_DeferredSale,
                    Pos_EditingOnDate = generalSettings.Pos_EditingOnDate,
                    Pos_ExceedDiscountRatio = generalSettings.Pos_ExceedDiscountRatio,
                    Pos_ModifyPricesType = (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4,
                    Pos_ExtractWithoutQuantity = generalSettings.Pos_ExtractWithoutQuantity,
                    Pos_IndividualCoding = generalSettings.Pos_IndividualCoding,
                    Pos_ModifyPrices = generalSettings.Pos_ModifyPrices,
                    Pos_PreventEditingRecieptFlag = generalSettings.Pos_PreventEditingRecieptFlag,
                    Pos_PreventEditingRecieptValue = generalSettings.Pos_PreventEditingRecieptValue,
                    Pos_PriceIncludeVat = generalSettings.Pos_PriceIncludeVat,
                    Pos_PrintPreview = generalSettings.Pos_PrintPreview,
                    Pos_PrintWithEnding = generalSettings.Pos_PrintWithEnding,
                    Pos_UseLastPrice = generalSettings.other_ActivatePriceLists && generalSettings.PriceListType == (int)PriceListsType.LastPriceForCustomer,
                    CustomerDisplay_Active = generalSettings.CustomerDisplay_Active,
                    CustomerDisplay_CharNumber = generalSettings.CustomerDisplay_CharNumber,
                    CustomerDisplay_Code = generalSettings.CustomerDisplay_Code,
                    CustomerDisplay_DefaultWord = generalSettings.CustomerDisplay_DefaultWord,
                    CustomerDisplay_LinesNumber = generalSettings.CustomerDisplay_LinesNumber,
                    CustomerDisplay_PortNumber = generalSettings.CustomerDisplay_PortNumber,
                    CustomerDisplay_ScreenType = generalSettings.CustomerDisplay_ScreenType,
                    
                },
                POSuserSettings = new UserSettings()
                {
                    posAddDiscount = userInfo.otherSettings.posAddDiscount,
                    posAllowCreditSales = userInfo.otherSettings.posAllowCreditSales,
                    posCashPayment = userInfo.otherSettings.posCashPayment,
                    posEditOtherPersonsInv = userInfo.otherSettings.posEditOtherPersonsInv,
                    posNetPayment = userInfo.otherSettings.posNetPayment,
                    posOtherPayment = userInfo.otherSettings.posOtherPayment,
                    posShowOtherPersonsInv = userInfo.otherSettings.posShowOtherPersonsInv,
                    posShowReportsOfOtherPersons = userInfo.otherSettings.salesShowReportsOfOtherPersons,

                },

                OtherSettings = new Other()
                {
                    Other_MergeItems = generalSettings.Other_MergeItems,
                    otherMergeItemMethod = generalSettings.otherMergeItemMethod,
                    Other_ItemsAutoCoding = generalSettings.Other_ItemsAutoCoding,
                    Other_ZeroPricesInItems = generalSettings.Other_ZeroPricesInItems,
                    Other_PrintSerials = generalSettings.Other_PrintSerials,
                    Other_AutoExtractExpireDate = generalSettings.Other_AutoExtractExpireDate,
                    Other_ViewStorePlace = generalSettings.Other_ViewStorePlace,
                    Other_ConfirmeSupplierPhone = generalSettings.Other_ConfirmeSupplierPhone,
                    Other_ConfirmeCustomerPhone = generalSettings.Other_ConfirmeCustomerPhone,
                    Other_DemandLimitNotification = generalSettings.Other_DemandLimitNotification,
                    Other_ExpireNotificationFlag = generalSettings.Other_ExpireNotificationFlag,
                    Other_ExpireNotificationValue = generalSettings.Other_ExpireNotificationValue,
                    Other_Decimals = generalSettings.Other_Decimals,
                    Other_ShowBalanceOfPerson = generalSettings.Other_ShowBalanceOfPerson
                },

                VatSettings = new VAT()
                {
                    Vat_Active = generalSettings.Vat_Active,
                    Vat_DefaultValue = generalSettings.Vat_DefaultValue,
                }

            };
            if (defaultStore != null)
            {
                res.storeId = defaultStore.Id;
                res.storeCode = defaultStore.Code;
                res.storeAr = defaultStore.ArabicName;
                res.storeEn = defaultStore.LatinName;
            }

            return new ResponseResult { Data = res, Result = Result.Success };
        }
        public async Task<bool> checkDeleteOfInvoice(int InvoiceTypeId, bool IsAccredite, bool IsReturn, bool IsDeleted
                  , string InvoiceType, int StoreId, DateTime InvoiceDate, int InvoiceId)
        {
            bool CanDelete = true;
            // incase of aaccredited
            if (((Lists.salesInvoicesList.Contains(InvoiceTypeId) ||
                Lists.purchasesInvoicesList.Contains(InvoiceTypeId))
                && IsAccredite)
                 || IsReturn || IsDeleted)
            {
                CanDelete = false;
            }
            // check serials
            else if (Lists.salesInvoicesList.Contains(InvoiceTypeId) || Lists.POSInvoicesList.Contains(InvoiceTypeId)
                  || Lists.ExtractPermissionInvoicesList.Contains(InvoiceTypeId))
            {

                List<string> serialsList = serialTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == InvoiceType
                    || a.ExtractInvoice == InvoiceType).Select(a => a.SerialNumber).ToList();
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
            else if (Lists.purchasesInvoicesList.Contains(InvoiceTypeId) || Lists.AddPermissionInvoicesList.Contains(InvoiceTypeId)
                || Lists.ItemsFundList.Contains(InvoiceTypeId))
            {
                // استرجاع بدون رصيد
                var PurchasesReturnWithoutQuantity = GeneralSettings.TableNoTracking.Select(a => a.Purchases_ReturnWithoutQuantity).First();

                var details = invoiceDetailsQuery.TableNoTracking
                       .Where(q => q.InvoiceId == InvoiceId && q.ItemTypeId != (int)ItemTypes.Service && q.ItemTypeId != (int)ItemTypes.Composite
                         && ((PurchasesReturnWithoutQuantity == true && q.ItemTypeId != (int)ItemTypes.Expiary) ? false : q.Quantity > 
                         CalcItemQuantity(0, q.ItemId, q.UnitId, StoreId, "", q.ExpireDate, false, 0, InvoiceDate,null,q.ItemTypeId).StoreQuantity));



                List<string> serialsList = serialTransactionQuery.TableNoTracking.Where(a => (a.AddedInvoice == InvoiceType
                          || a.ExtractInvoice == InvoiceType) && a.IsDeleted == false).Select(a => a.SerialNumber).ToList();
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

        #region functions


        #region َQuantity
        // تتنفذ في حالة الصنف المركب بس فى فواتير الصرف
        public QuantityInStoreAndInvoice  CalcCompositeItemQuantity(int? invoiceId, int ItemId, int? UnitId, int StoreId, string ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared, int? invoiceTypeId, DateTime invoiceDate, List<CalcQuantityRequest> items, int itemTypeId)
        {
            var QuantityInStoreAndInvoice = new QuantityInStoreAndInvoice();

            var itemdata_ = itemCardMasterRepository.TableNoTracking.Include(a=>a.Parts).Where(a => a.Id == ItemId);
            if(itemdata_.Count()>0)
            {
                var selectedFactor = itemUnitsQuery.TableNoTracking.Where(a => a.ItemId == ItemId && a.UnitId == UnitId)
                              .Select(a => a.ConversionFactor).FirstOrDefault();

                var itemdata = itemdata_.First();
                if(itemdata.TypeId!=(int)ItemTypes.Composite)
                    return  null;
                double minimumStoreQuantityWithOutInvoice = 0.0;
                double minimumStoreQuantity = 0.0;
                double minimumInvoiceQuantity = 0.0;
               foreach(var item in itemdata.Parts)
                {
                    
                    var qty = CalcItemQuantityNotComposite(invoiceId, item.PartId, item.UnitId, StoreId, ParentInvoiceType, null, false,
                                               invoiceTypeId, invoiceDate, items, (int)ItemTypes.Store);// هنا ببعت الصنف مخزني لان مكونات الصنف المركب مخزني بس

                    minimumStoreQuantityWithOutInvoice = minimumQtyOfCompositeItem(qty.StoreQuantityWithOutInvoice, item.Quantity, minimumStoreQuantityWithOutInvoice);
                    minimumStoreQuantity = minimumQtyOfCompositeItem(qty.StoreQuantity, item.Quantity, minimumStoreQuantity);
                    minimumInvoiceQuantity = minimumQtyOfCompositeItem(qty.InvoiceQuantity, item.Quantity, minimumInvoiceQuantity);
                }
                int signal = GetSignal(invoiceTypeId.Value);
                double enteredQuantity = (items == null ? 0 : CalcQuantityFromRequest(items, ItemId, selectedFactor, signal,itemTypeId));

                QuantityInStoreAndInvoice.StoreQuantityWithOutInvoice = minimumStoreQuantityWithOutInvoice+ enteredQuantity;
                QuantityInStoreAndInvoice.StoreQuantity = minimumStoreQuantity;
                QuantityInStoreAndInvoice.InvoiceQuantity = minimumInvoiceQuantity;
            }
            return QuantityInStoreAndInvoice;
        }
           private double minimumQtyOfCompositeItem(double ComponentQtyInDB , double ComponentQtyInItem , double minimumQty)
        {
            double  availableQuantity = ComponentQtyInDB / ComponentQtyInItem;


            if (minimumQty > availableQuantity || minimumQty == 0)
                minimumQty = availableQuantity;
            return minimumQty;
        }

        // حساب كميات الصنف الحالى فى باقي الفاتورة
           public double CalcQuantityFromRequest(List<CalcQuantityRequest> items, int currentItemId, double currentFactor, int signal , int currentItemTypeId)
        {
            // get componnet of composite item
            if(currentItemTypeId != (int)ItemTypes.Composite) // فى حالة الصنف الحالى مش مركب -> هجيب مكونات الاصناف المركبه المبعوته فى باقي الفاتورة
            {
                var compositeItems = items.Where(a => a.itemTypeId == (int)ItemTypes.Composite);
                if (compositeItems.Count() > 0)
                {
                    List<CalcQuantityRequest> componnentItemInRequest = new List<CalcQuantityRequest>();
                    foreach (var item in compositeItems)
                    {
                        var componnentItems = setCompositItem(item.itemId, 0, item.enteredQuantity);

                        componnentItems.ForEach(a =>
                                componnentItemInRequest.Add(new CalcQuantityRequest()
                                { itemId = a.ItemId, conversionFactor = a.ConversionFactor, enteredQuantity = a.Quantity, itemTypeId = (int)ItemTypes.Store }));
                    }
                    items.AddRange(componnentItemInRequest);
                    items.RemoveAll(a => a.itemTypeId == (int)ItemTypes.Composite); //loop بمسح كل الاصناف المركبه بعد اما افكها لمنع تكرار الفك فى ال 
                }
            }
        
            var enteredQuantity = items.Where(a => a.itemId == currentItemId).Sum(a => a.enteredQuantity * signal * a.conversionFactor / currentFactor);
            return enteredQuantity;
        }
        public QuantityInStoreAndInvoice CalcItemQuantity(int? invoiceId, int ItemId, int? UnitId, int StoreId, string ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared, int? invoiceTypeId, DateTime invoiceDate, List<CalcQuantityRequest> items , int itemTypeId)
        {
            if(itemTypeId==0)
            {
                itemTypeId = itemCardMasterRepository.TableNoTracking.Where(a => a.Id == ItemId).First().TypeId;
            }
            if (Lists.CompositeItemOnInvoice.Contains(invoiceTypeId.Value))
            {
                var composieItem = CalcCompositeItemQuantity(invoiceId, ItemId, UnitId, StoreId, ParentInvoiceType, null, false, invoiceTypeId, invoiceDate, items,itemTypeId);
                if (composieItem != null)
                    return composieItem;
            }

            return CalcItemQuantityNotComposite(invoiceId, ItemId, UnitId, StoreId, ParentInvoiceType, ExpiryDate, IsExpiared, invoiceTypeId, invoiceDate, items, itemTypeId);


        }
        public QuantityInStoreAndInvoice CalcItemQuantityNotComposite(int? invoiceId, int ItemId, int? UnitId, int StoreId, string ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared, int? invoiceTypeId, DateTime invoiceDate, List<CalcQuantityRequest> items, int itemTypeId)
        {
            // invoiceId  دعشان اجيب الكميات بتاعت الصنف ف حاله التعديل ف استثني الفاتوره اللي بعدلها 
            var QuantityInStoreAndInvoice = new QuantityInStoreAndInvoice();
            var setingOfDecimal = GeneralSettings.TableNoTracking.First().Other_Decimals;
            // convert quantities to the selected unit in invoice
            var selectedFactor = itemUnitsQuery.TableNoTracking.Where(a => a.ItemId == ItemId && a.UnitId == UnitId)
                                .Select(a => a.ConversionFactor).FirstOrDefault();
            if (selectedFactor == 0) // if itemId or unitId doesn't exist
            {
                return QuantityInStoreAndInvoice;
            }

            // calc quantity in store
            // QUANTITY IN STOER + invoice
            var TotalQuantity = invoiceDetailsQuery.TableNoTracking
                                        .Include(a => a.InvoicesMaster)
                                        .Where(a => a.ItemId == ItemId && a.InvoicesMaster.StoreId == StoreId
                                              && (ExpiryDate != null ? a.ExpireDate == ExpiryDate : true) //calculate quantity of specific expiared date
                                              && (a.ItemTypeId == (int)ItemTypes.Expiary ? (!IsExpiared ? true : a.ExpireDate > invoiceDate.Date) : true)); //IsExpiared=false calculate total quantity of not expiared dates 
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
            if (invoiceId > 0)
            {
                var mainInvoice = invoiceMasterQuery.TableNoTracking.Where(a => a.InvoiceId == invoiceId.Value).Select(a => new { a.InvoiceTypeId, a.InvoiceType }).ToList().First();
                TotalQuantity = TotalQuantity.Where(a => a.InvoicesMaster.InvoiceType == mainInvoice.InvoiceType || a.InvoicesMaster.ParentInvoiceCode == mainInvoice.InvoiceType);
                TotalQuantityInSmallUnit = TotalQuantity.Sum(a => a.Quantity * a.Signal * a.ConversionFactor);

                totalQty = TotalQuantityInSmallUnit / selectedFactor;
                invoiceQuantity = totalQty;
                QuantityInStoreAndInvoice.InvoiceQuantity = Math.Round(Math.Abs(totalQty), setingOfDecimal);

            }
            // الكمية فى المخزن 
            int signal = GetSignal(invoiceTypeId.Value);
            double enteredQuantity = (items == null ? 0 : CalcQuantityFromRequest(items, ItemId, selectedFactor, signal , itemTypeId));
            QuantityInStoreAndInvoice.StoreQuantityWithOutInvoice = Math.Round(QuantityInStoreAndInvoice.StoreQuantity
                                                                               - invoiceQuantity + enteredQuantity, setingOfDecimal);

            return QuantityInStoreAndInvoice;
        }

        public List<InvoiceDetailsRequest> setCompositItem(int itemId, int unitId, double qty)
        {
            var compositeItems = new List<CompositeItemsRequest>();
            compositeItems.Add(
                new CompositeItemsRequest() { itemId = itemId, unitId = unitId, quantity = qty });
            return setCompositItem(compositeItems,null);

        }

        public List<compositItem> GetComponentsOfCompositItem(List<CompositeItemsRequest> compositeRequest )
        {

            var itemData = itemCardPartsQuery.TableNoTracking.Include(a => a.CardMaster).Include(a => a.Unit)
                .Where(a => compositeRequest.Select(e => e.itemId).Contains(a.ItemId));
            List<compositItem> itemData_ = new List<compositItem>();
            foreach (var item in compositeRequest)
            {
                var itemParts = itemData.Where(a => a.ItemId == item.itemId);
                foreach (var part in itemParts)
                {
                    itemData_.Add(new compositItem()
                    {
                        ItemId = part.ItemId,
                        PartId = part.PartId,
                        UnitId = part.UnitId,
                        Quantity = part.Quantity,
                        invoiceId = item.invoiceId,
                        indexOfItem = item.indexOfItem
                    });
                }

            }
            return itemData_;
        }
        public List<InvoiceDetailsRequest> setCompositItem(List<CompositeItemsRequest> compositeRequest, List<compositItem> compositItems)
        {
            var componentItems = new List<InvoiceDetailsRequest>();
            // var itemData = itemCardPartsQuery.TableNoTracking.Include(a => a.CardMaster).Include(a => a.Unit)
            //.Where(a => compositeRequest.Select(e => e.itemId).Contains(a.ItemId));
            var itemData_ = new List<compositItem>();
            if (compositItems == null)  // call in online mode here and call it in offline mode at offlineHander to use it there 
                itemData_ = GetComponentsOfCompositItem(compositeRequest);
            else
                itemData_ = compositItems;
            var unitsOfComponnent = itemData_.Select(e => e.PartId).ToList();
            var itemunit = itemUnitsQuery.TableNoTracking.Where(a => unitsOfComponnent.Contains(a.ItemId)).ToList();
            foreach (var item in itemData_)
            {
                var componentItem = new InvoiceDetailsRequest();
                componentItem.InvoiceId = item.invoiceId;
                componentItem.ItemId = item.PartId;
                componentItem.UnitId = item.UnitId;
                componentItem.Quantity = compositeRequest.First(a => a.itemId == item.ItemId &&
                                       a.indexOfItem== item.indexOfItem && a.invoiceId== item.invoiceId).quantity * item.Quantity;
                //componentItem.ItemTypeId = item.CardMaster.TypeId;
                // componentItem.ItemCode = item.CardMaster.ItemCode;
                componentItem.parentItemId = compositeRequest.First(a => a.itemId == item.ItemId).itemId;
                componentItem.ConversionFactor = itemunit.Where(a => a.UnitId == item.UnitId).First().ConversionFactor;
                componentItem.IndexOfItem = item.indexOfItem;
                //  var itemDetails = ItemDetails(invoice, componentItem);
                componentItem.Price = itemunit.Where(a => a.UnitId == item.UnitId).First().SalePrice1; // to avoid divide on 0 in discount ratio
                componentItems.Add(componentItem);
            }
            return componentItems;
        }
        #endregion



        public serialsReponse CheckSerial(serialsRequest request, bool fromSavingInvoice)
        {
            var newSerials = new List<string>();
            if (request.isDiffNumbers) // لو السريالات مختلفه
            {

                request.serial = request.serial.Trim().ToUpper();
                if (request.serial == "")
                    return new serialsReponse
                    {
                        InvoiceSerialDtos = request.newEnteredSerials,
                        serialsCount = request.newEnteredSerials.Count(),
                        serialsStatus = SerialsStatus.requiredSerial
                    };
                newSerials.Add(request.serial);

            }
            else  // لو السيريالات متتابعه
            {
                if (request.stratPattern != null)
                    request.stratPattern = request.stratPattern.Trim().ToUpper();
                if (request.endPattern != null)
                    request.endPattern = request.endPattern.Trim().ToUpper();

                // required period
                if (request.fromNumber == 0 || request.toNumber == 0 || request.fromNumber == null || request.toNumber == null)
                    return new serialsReponse
                    {
                        InvoiceSerialDtos = request.newEnteredSerials,
                        serialsCount = request.newEnteredSerials.Count(),
                        serialsStatus = SerialsStatus.requiredPeriod,
                        ErrorMessageAr = "!يجب ادخال المقطع الاوسط كامل",
                        ErrorMessageEn = "The middle section must be entered completely"
                    };

                if (request.fromNumber > request.toNumber)
                    return new serialsReponse
                    {
                        InvoiceSerialDtos = request.newEnteredSerials,
                        serialsCount = request.newEnteredSerials.Count()
                        ,
                        serialsStatus = SerialsStatus.errorInPeriod,
                        ErrorMessageAr = "!يجب ادخال الأرقام بشكل صحيح",
                        ErrorMessageEn = "Enter numbers correctly"
                    };

                var current = request.fromNumber;
                int counter = 1;
                // loop for creating serials and make concatenate with patterns
                while (current <= request.toNumber)
                {
                    if (counter > (int)SerialsStatus.limitOfSerialsCount) // this limit for avoid  over loading
                        return new serialsReponse
                        {
                            InvoiceSerialDtos = request.newEnteredSerials,
                            serialsCount = request.newEnteredSerials.Count(),
                            serialsStatus = SerialsStatus.limitOfSerialsCount,
                            ErrorMessageAr = " الحد الاقصى لادخال السيريالات " + (int)SerialsStatus.limitOfSerialsCount,
                            ErrorMessageEn = "The maximum number of serials is " + (int)SerialsStatus.limitOfSerialsCount
                        };

                    newSerials.Add(request.stratPattern + current + request.endPattern);
                    current++;
                    counter++;
                }
            }


            var repeatedSerials = new List<string>();

            // compare with serials in request
            if (!fromSavingInvoice)
            {
                repeatedSerials = request.newEnteredSerials.Select(a => a.SerialNumber).Intersect(newSerials).ToList();
                if (repeatedSerials.Count() > 0)
                    return new serialsReponse
                    {
                        InvoiceSerialDtos = request.newEnteredSerials,
                        serialsCount = request.newEnteredSerials.Count(),
                        serialsStatus = SerialsStatus.repeatedInCurrentSerials,
                        ErrorMessageAr = "متكرر في السيريالات الحالية",
                        ErrorMessageEn = " Repeated in current serials"
                    };
                // compare with the same invoice
                var serialsInTheSameInvoice = request.serialsInTheSameInvoice.Replace("[", "").Replace("]", "");
                var serialsInTheSameInvoiceList = new List<string>();
                serialsInTheSameInvoiceList = serialsInTheSameInvoice.Split(',').ToList();
                repeatedSerials = serialsInTheSameInvoiceList.Intersect(newSerials).ToList();
                if (repeatedSerials.Count() > 0)
                    return new serialsReponse
                    {
                        InvoiceSerialDtos = request.newEnteredSerials,
                        serialsCount = request.newEnteredSerials.Count(),
                        serialsStatus = SerialsStatus.repeatedInThisInvoice,
                        ErrorMessageAr = "متكرر ف الاصناف الحاليه",
                        ErrorMessageEn = " Repeated in currnet items"
                    };

            }


            // compare with old serials

            var oldSerials_ = serialTransactionQuery.TableNoTracking.Where(a =>
                                (string.IsNullOrEmpty(a.ExtractInvoice) || a.TransferStatus == TransferStatus.Binded) &&
                                (!string.IsNullOrEmpty(request.invoiceType) ? a.AddedInvoice != request.invoiceType : true) && a.IsDeleted == false);

            // السيريالت فى قيد التحويل
            var serialsBinded = oldSerials_.Where(a => a.TransferStatus == TransferStatus.Binded).Select(a => a.SerialNumber).ToList();
            repeatedSerials = serialsBinded.Intersect(newSerials).ToList();
            if (repeatedSerials.Count() > 0)
                return new serialsReponse
                {
                    InvoiceSerialDtos = request.newEnteredSerials,
                    serialsCount = request.newEnteredSerials.Count(),
                    serialsStatus = SerialsStatus.bindedTransfer,
                    ErrorMessageAr = ErrorMessagesAr.SerialsBinded,
                    ErrorMessageEn = ErrorMessagesEn.SerialsBinded
                };

            // السيريالات موجوده فى الداتابيز
            var oldSerials = oldSerials_.Where(a => a.TransferStatus != TransferStatus.Binded).Select(a => a.SerialNumber).ToList();
            repeatedSerials = oldSerials.Intersect(newSerials).ToList();
            if (repeatedSerials.Count() > 0)
                return new serialsReponse
                {
                    InvoiceSerialDtos = request.newEnteredSerials,
                    serialsCount = request.newEnteredSerials.Count(),
                    serialsStatus = SerialsStatus.repeatedInOldSerials,
                    ErrorMessageAr = "متكرر فى السيريالات القديمة",
                    ErrorMessageEn = "Repeated in old serials"
                };

            // compare with barcode in item card
            List<string> NationalBarcode = itemCardMasterRepository.TableNoTracking.Where(a => a.NationalBarcode != null)
                                              .Select(a => a.NationalBarcode).ToList();
            List<string> Barcode = itemUnitsQuery.TableNoTracking.Where(a => a.Barcode != null)
                                        .Select(a => a.Barcode).ToList();
            List<string> itemCode = itemCardMasterRepository.TableNoTracking.Select(a => a.ItemCode).ToList();
            // merge lists in one list
            Barcode.AddRange(itemCode);
            Barcode.AddRange(NationalBarcode);

            repeatedSerials = Barcode.Intersect(newSerials).ToList();
            if (repeatedSerials.Count() > 0)
                return new serialsReponse
                {
                    InvoiceSerialDtos = request.newEnteredSerials,
                    serialsCount = request.newEnteredSerials.Count(),
                    serialsStatus = SerialsStatus.repeatedInItems,
                    ErrorMessageAr = "متكرره فى كود الأصناف او الباركود",
                    ErrorMessageEn = "Repeated in items code or barcode"
                };

            // accept new serials
            //    request.newEnteredSerials.Select(a=>a.SerialNumber).ToList().InsertRange(0,newSerials);
            newSerials.ForEach(a => request.newEnteredSerials.Add(new InvoiceSerialDto() { CanDelete = true, SerialNumber = a }));

            // request.newEnteredSerials.AddRange(newSerials);
            return new serialsReponse { InvoiceSerialDtos = request.newEnteredSerials, serialsCount = request.newEnteredSerials.Count(), serialsStatus = SerialsStatus.accepted };

        }
        //private static void AddMergeItem(List<InvoiceDetailsRequest> listDto, InvoiceDetailsRequest item, InvoiceDetailsRequest mergeItemDto)
        //{
        //    mergeItemDto.ItemId = item.ItemId;
        //    mergeItemDto.ItemCode = item.ItemCode;
        //    //  mergeItemDto.ItemNameAr = item.ItemNameAr;
        //    //  mergeItemDto.ItemNameEn = item.ItemNameEn;
        //    mergeItemDto.UnitId = item.UnitId;
        //    mergeItemDto.SizeId = item.SizeId;
        //    // mergeItemDto.UnitNameAr = item.UnitNameAr;
        //    //  mergeItemDto.UnitNameEn = item.UnitNameEn;
        //    mergeItemDto.Quantity = item.Quantity;
        //    mergeItemDto.Price = item.Price;
        //    mergeItemDto.ApplyVat = item.ApplyVat;
        //    mergeItemDto.VatRatio = item.VatRatio;
        //    mergeItemDto.VatValue = item.VatValue;
        //    mergeItemDto.DiscountRatio = item.DiscountRatio;
        //    mergeItemDto.DiscountValue = item.DiscountValue;
        //    mergeItemDto.TransQuantity = item.TransQuantity;
        //    mergeItemDto.TransStatus = item.TransStatus;
        //    mergeItemDto.ExpireDate = item.ExpireDate;
        //    mergeItemDto.ItemTypeId = item.ItemTypeId;
        //    mergeItemDto.AutoDiscount = item.AutoDiscount;
        //    mergeItemDto.SplitedDiscountValue = item.SplitedDiscountValue;
        //    mergeItemDto.SplitedDiscountRatio = item.SplitedDiscountRatio;
        //    mergeItemDto.ConversionFactor = item.ConversionFactor;
        //    mergeItemDto.Total += item.Total;
        //    mergeItemDto.parentItemId = item.parentItemId;

        //    if (item.ItemTypeId == (int)ItemTypes.Serial)
        //    {
        //        var serialList = new List<string>();
        //        foreach (var serials in item.ListSerials)
        //        {

        //            //var serial = new ListOfSerials();
        //            //serial.SerialNumber = serials ;

        //            serialList.Add(serials);

        //        }
        //        mergeItemDto.ListSerials.AddRange(serialList);

        //    }
        //    listDto.Add(mergeItemDto);

        //}
        public double CreateSerializeOfInvoice(int invoiceType, int invoiceId, int branchId)
        {
            double serialize = 1;
            var serializeData = invoiceSerializeQuery.TableNoTracking.ToList().Count();

            if (serializeData > 0)
            {
                if (Lists.MainInvoiceList.Contains(invoiceType) || ((Lists.returnInvoiceList.Contains(invoiceType)) && invoiceId == 0)) // مرتجع بدون فاتورة
                {
                    int serial = (int)invoiceSerializeQuery.GetMaxCode(a => a.Serialize) + 1;
                    serialize = (double)serial;
                }
                else
                {
                    serialize = invoiceSerializeQuery.TableNoTracking.Where(a => a.InvoiceCode == invoiceId && a.BranchId == branchId).ToList().Select(a => a.Serialize).Max() + 0.01;
                }

            }

            return Math.Round(serialize, 2);
        }
        public bool addSerialize(double serialize, int MainInvoiceId, int invoiceType, int branchId)
        {

            var addSerialize_ = new InvoiceSerialize
            {
                Serialize = serialize,
                InvoiceCode = MainInvoiceId,
                InvoiceTypeId = invoiceType,
                BranchId = branchId
            };

            bool saved = invoiceSerializeCommand.Add(addSerialize_);


            return saved;
        }
        // Check invoice existance
        // this api to avoid sending data in routing for front
        public async Task<ResponseResult> CheckInvoiceExistance(string invoiceType, int InvoiceTypeId)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            try
            {
                var invoiceExist = await invoiceMasterQuery.TableNoTracking.FirstOrDefaultAsync(q => ((q.InvoiceType == invoiceType || q.Code.ToString() == invoiceType) && q.InvoiceTypeId == InvoiceTypeId && q.BranchId == userInfo.CurrentbranchId)
                                    && Lists.MainInvoiceForReturn.Contains(q.InvoiceTypeId));

                return new ResponseResult
                {
                    Result = invoiceExist == null ? Result.NoDataFound :
                         (invoiceExist.InvoiceSubTypesId == (int)SubType.TotalReturn ? Result.InvoiceTotalReturned : (invoiceExist.IsDeleted ? Result.Deleted : Result.Success))
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult
                {
                    Note = ex.Message,
                    Result = Result.Failed
                };
            }
        }
        public async Task<ResponseResult> SetQuantityForExpiaryDate(SetQuantityForExpiaryDateRequest request)
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
            var TotalQuantity = CalcItemQuantity(request.invoiceId, request.itemId, request.unitId, request.storeId, "", null, false, request.invoiceTypeId, request.InvoiceDate,null,0).StoreQuantityWithOutInvoice;
            // check if 
            if (TotalQuantity < (request.quantity + oldTotalQuantity))
                return new ResponseResult { Data = null, Result = Result.QuantityNotavailable, Note = "QuantityNotavailable"
                            , ErrorMessageAr =ErrorMessagesAr.QuantityNotAvailableForItem,ErrorMessageEn=ErrorMessagesEn.QuantityNotAvailableForItem};


            var expiaryList = new List<ExpiaryData>();
            if (expiaryOfInvoice.Count() > 0)
            {
                var qty = request.quantity;
                var setDecimal = GeneralSettings.TableNoTracking.Select(a => a.Other_Decimals).First();

                foreach (var expiary in expiaryOfInvoice)
                {
                    if (qty == 0) break;
                    var currentQuantity = CalcItemQuantity(request.invoiceId, request.itemId, request.unitId, request.storeId, ""
                                   , expiary.Key, true, request.invoiceTypeId, request.InvoiceDate,null,0).StoreQuantityWithOutInvoice;
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
        public async Task<bool> generateEditedItems(List<InvoiceDetails> invoiceDetailsList, double serialize, bool isUpdate, int invoiceId,int branchId)
        {
            var ItemsHaveNoEffictOnInvoice = new List<int> { (int)ItemTypes.Additives, (int)ItemTypes.Note };
            var itemsForEditedItems = new List<editedItemsParameter>();
            var itemsList = invoiceDetailsList.Select(a => new { itemId = a.ItemId, itemTypeId = a.ItemTypeId, serialize = serialize, qty = a.Quantity, price = a.Price ,BranchId = branchId })
                .Where(a =>  !ItemsHaveNoEffictOnInvoice.Contains(a.itemTypeId)).ToList();

            if (isUpdate) // compare items between old invoice and new
            {
                var oldItems = invoiceDetailsQuery.TableNoTracking.Where(a => a.InvoiceId == invoiceId && !ItemsHaveNoEffictOnInvoice.Contains(a.ItemTypeId))
                      .Select(a => new { itemId = a.ItemId, itemTypeId = a.ItemTypeId, serialize = serialize, qty = a.Quantity, price = a.Price ,BranchId= branchId }).ToList();
                var removedItems = oldItems.Except(itemsList);
                //var modifiedItems = oldItems
                //    .Join(
                //    itemsList,
                //    a => a.itemId,
                //    p => p.itemId,
                //    (oItems, nItems) => new { oItems, nItems }).Where(h => h.oItems.price != h.nItems.price || h.oItems.qty != h.nItems.qty)
                //    .Select(a => new { itemId = a.nItems.itemId, itemTypeId = a.nItems.itemTypeId, serialize = serialize, qty = a.nItems.qty, price = a.nItems.price }).ToList();

                if (removedItems.Any())
                {
                    itemsList.AddRange(removedItems);
                }
                // itemsList = modifiedItems;
            }
            var itemsList_ = itemsList.GroupBy(a => new { a.itemId, a.itemTypeId, a.serialize }).ToList();
            itemsList_.ForEach(a =>
                     itemsForEditedItems.Add
                     ( new editedItemsParameter
                     {
                         itemId = a.Key.itemId,
                         itemTypeId = a.Key.itemTypeId,
                         sizeId = 0,
                         serialize =serialize,
                         branchId = branchId
                     }
                     )
                     );
            //itemsForEditedItems.serialize = serialize;
            //itemsForEditedItems.branchId = branchId;
              await editedItemsService.AddItemInEditedItem(itemsForEditedItems);
            return true;
        }
        public async Task<ResponseResult> GetTotalAmountOfPerson(int? personId, int? FinancialAccountId)
        {
            var person = PersonRepositorQuery.TableNoTracking.Where(x => x.Id == personId);
            if (!person.Any())
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            var totalAmount = 0;/*await _receiptsService.GetReceiptCurrentFinancialBalance(person.FirstOrDefault().IsCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, person.FirstOrDefault().Id);*/
            return new ResponseResult()
            {
                Data = totalAmount
            };
        }
        public int GetSignal(int invoiceTypeId)
        {
            int signal = 1;
            var invoicesTypes = new List<int> { (int)DocumentType.DeletePurchase, (int)DocumentType.ReturnPurchase
                                            ,(int)DocumentType.POS , (int)DocumentType.OutgoingTransfer ,(int)DocumentType.Sales 
                                            ,(int)DocumentType.DeleteAddPermission,(int)DocumentType.DeleteItemsFund
                                            ,(int)DocumentType.ExtractPermission,(int)DocumentType.SafePayment
                                            ,(int)DocumentType.BankPayment,(int)DocumentType.CompinedSafePayment
                                            ,(int)DocumentType.CompinedBankPayment,(int)DocumentType.PermittedDiscount
                                            ,(int)DocumentType.OfferPrice ,(int)DocumentType.ReturnWov_purchase
                                            ,(int)DocumentType.DeleteWov_purchase,(int)DocumentType.BankTransfer
                                            ,(int)DocumentType.SafeTransfer};

            if (invoicesTypes.Contains(invoiceTypeId))
                signal = -1;

            return signal;
        }
     
        public async Task<Tuple<bool, string, string>> checkQuantityBeforeSaveInvoiceForExtract(List<InvoiceDetailsRequest> invoiceDetails, int storeId, DateTime invoiceDate, int invoiceId, InvGeneralSettings? setting, int invoiceTypeId, int signal, int oldStoreId, bool isExpired,string invoiceType)
        {
            var itemsNotCheckedQty = new List<int> { (int)ItemTypes.Serial, (int)ItemTypes.Service, (int)ItemTypes.Note , (int)ItemTypes.Composite };
            if (invoiceTypeId == (int)DocumentType.ExtractPermission)  // في اذن الصرف بصرف التاريخ المنتهي 
                isExpired = true;
            //1
            var currentInvoice = invoiceDetails.Where(a => !itemsNotCheckedQty.Contains(a.ItemTypeId))
                .GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                .Select(a => new { a.Key.SizeId, a.First().ItemTypeId, a.First().ItemCode, a.Key.ItemId, 
                             qty = a.Sum(e => e.Quantity * e.ConversionFactor), a.Key.ExpireDate , a.First().parentItemId}).ToList();
            var ItemsNotAvailable = new List<int>();
            var expiaryItemsNotAvailable = new List<string>();
            bool storeHasQty = true;
            bool checkQuantity = true;

            if (Lists.InvoicesTypeForExtractStore_Setting.Contains(invoiceTypeId))
                checkQuantity = setting.Sales_ExtractWithoutQuantity;
            else if (Lists.InvoicesTypeForAddStore_Setting.Contains(invoiceTypeId))
                checkQuantity = setting.Purchases_ReturnWithoutQuantity;
            else if (Lists.POSInvoicesList.Contains(invoiceTypeId))
                checkQuantity = setting.Pos_ExtractWithoutQuantity;
            else if (Lists.transferStore.Contains(invoiceTypeId))
                checkQuantity = false;

            bool hasExpirydates = false;
            if (checkQuantity)
            {
                var ExpiaryInCurrentInvoice = currentInvoice.Where(a => a.ItemTypeId == (int)ItemTypes.Expiary ||( a.parentItemId!=null && a.parentItemId!=0)).ToList();
                if (ExpiaryInCurrentInvoice.Count() > 0)
                {

                    currentInvoice = ExpiaryInCurrentInvoice;
                    hasExpirydates = true;
                }
            }
            if (!checkQuantity || hasExpirydates)
            {

                var QuantityDto = new List<QuantityDto>();
                if (invoiceId > 0 && !Lists.deleteInvoiceExtractFromStore.Contains(invoiceTypeId) && !Lists.returnInvoiceList.Contains(invoiceTypeId))
                {
                    //2 
                    var mainInvoice = invoiceDetailsQuery.TableNoTracking
                        .Where(a => a.InvoiceId == invoiceId && a.InvoicesMaster.StoreId == storeId)//&& !itemsNotCheckedQty.Contains(a.ItemTypeId)&&!Lists.returnInvoiceList.Contains(invoiceTypeId))
                         .GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                    .Select(a => new { a.Key.SizeId, a.Key.ItemId, qty = a.Sum(e => e.Quantity * e.ConversionFactor), a.Key.ExpireDate }).ToList();

                    //3  DifQuantity

                    foreach (var item in currentInvoice)
                    {
                        var qtyOfMainInvoice = mainInvoice.Where(a => a.ItemId == item.ItemId && ((a.SizeId == null || a.SizeId == 0) ? true : a.SizeId == item.SizeId) &&
                                    (item.ItemTypeId == (int)ItemTypes.Expiary ?( item.ExpireDate == null ? true : a.ExpireDate == item.ExpireDate):true))
                            .Select(a => a.qty).ToList().Sum();
                        var diffQty = item.qty - qtyOfMainInvoice;
                        //if (diffQty < 0)
                        //    continue;
                        var Qty = new QuantityDto();
                        // Qty.itemCode=item.ItemCode;
                        Qty.qty = diffQty;
                        Qty.itemId = item.ItemId;
                        if (item.ItemTypeId ==(int)ItemTypes.Expiary)
                            Qty.expiryDate = item.ExpireDate.Value.Date;
                        QuantityDto.Add(Qty);
                    }
                }


                //4
                var qtyAvailableInStore = invoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster)
                    .Where(a => a.InvoicesMaster.StoreId == storeId && ((QuantityDto.Count() > 0 && invoiceId > 0 && !Lists.deleteInvoiceExtractFromStore.Contains(invoiceTypeId)
                                                                  && !Lists.returnInvoiceList.Contains(invoiceTypeId)) ?
                         QuantityDto.Select(e => e.itemId).Contains(a.ItemId) : currentInvoice.Select(e => e.ItemId).Contains(a.ItemId)) &&
                          (a.ExpireDate == null ? true : (isExpired ? true : a.ExpireDate.Value.Date > invoiceDate))).GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                       .Select(a =>
                        new
                        {
                            a.Key.SizeId,
                            itemName = a.First().Items.ArabicName,
                            a.Key.ItemId,
                            qty = a.Sum(e => e.Quantity * e.ConversionFactor * e.Signal),
                            a.Key.ExpireDate
                        }).ToList();



                if (qtyAvailableInStore.Count() == 0 && currentInvoice.Count > 0)
                {
                    storeHasQty = false;
                    //    ItemsNotAvailable = currentInvoice;
                }
                else
                {
                    // extract items not available
                    foreach (var item in qtyAvailableInStore)
                    {
                        var qtyOfItemInStore = item.qty;
                        var expDate = item.ExpireDate;
                        var itemid = item.ItemId;

                        var qtyOfItemInCurrentInvOice = ((invoiceId > 0 && !Lists.deleteInvoiceExtractFromStore.Contains(invoiceTypeId)
                                                     && !Lists.returnInvoiceList.Contains(invoiceTypeId)) ?
                                                  QuantityDto.Where(a => a.itemId == itemid && ((a.sizeId == null || a.sizeId == 0) ? true : a.sizeId == item.SizeId) &&
                                       a.expiryDate == expDate).Sum(a => a.qty) : currentInvoice.Where(a => a.ItemId == itemid &&
                                       ((a.SizeId == null || a.SizeId == 0) ? true : a.SizeId == item.SizeId)
                                       && (expDate != null ? a.ExpireDate.Value.Date == expDate : true)).Sum(a => a.qty));

                        if (qtyOfItemInStore < qtyOfItemInCurrentInvOice || (storeId != oldStoreId && qtyOfItemInStore == 0))
                         {
                          
                               ItemsNotAvailable.Add(item.ItemId);
                            if (expDate != null)
                                expiaryItemsNotAvailable.Add(item.ExpireDate.Value.Date.ToShortDateString());
                         }
                    }

                }
                //var invoiceCountSameInUpdate = true;//currentInvoice.Count();
                //if (invoiceId > 0 && !Lists.deleteInvoiceExtractFromStore.Contains(invoiceTypeId)
                //                                     && !Lists.returnInvoiceList.Contains(invoiceTypeId))
                //{
                //    if (currentInvoice.Count() != QuantityDto.Count())
                //        invoiceCountSameInUpdate = false;
                //}

                if (qtyAvailableInStore.Count()!= currentInvoice.Count() )
                {
                    var itemsNotInStore = currentInvoice.Where(a => !qtyAvailableInStore.Select(e => e.ItemId).Contains(a.ItemId)).Select(a => a.ItemId);
                 //   var itemsNotInStoreName = itemCardMasterRepository.TableNoTracking.Where(a => itemsNotInStore.Contains(a.Id)).Select(a => a.ArabicName).ToList();
                    ItemsNotAvailable.AddRange(itemsNotInStore);
                }

            }

            //5  check qty in invoice

            if (ItemsNotAvailable.Count() <= 0)
            {
                if (Lists.returnInvoiceList.Contains(invoiceTypeId) && !string.IsNullOrEmpty(invoiceType))
                {

                    var mainInvoice = invoiceDetailsQuery.TableNoTracking
                     .Where(a => a.InvoiceId == invoiceId)
                 .Select(a => new
                 {
                     a.SizeId,
                     a.ItemId,
                     qty =Math.Round( (a.Quantity * a.ConversionFactor) - (a.ReturnQuantity * a.ConversionFactor) , setting.Other_Decimals),
                     a.ExpireDate,
                     a.indexOfItem
                 }).ToList();

                    var _currentInvoice = invoiceDetails.Where(a => !itemsNotCheckedQty.Contains(a.ItemTypeId))
            .Select(a => new
            {
                a.SizeId,
                a.ItemTypeId,
                a.ItemCode,
                a.ItemId,
                qty = (a.Quantity * a.ConversionFactor),
                a.ExpireDate,
                a.IndexOfItem
            }).ToList();


                    foreach (var item in _currentInvoice)
                    {
                        var qtyInMainInvoice = mainInvoice.Where(a => a.ItemId == item.ItemId && a.indexOfItem == item.IndexOfItem &&
                        ((a.SizeId == null || a.SizeId == 0) ? true : a.SizeId == item.SizeId) &&
                                  (item.ItemTypeId != (int)ItemTypes.Expiary ? true : a.ExpireDate.Value.Date == item.ExpireDate.Value.Date)
                                  ).Sum(e => e.qty);
                        if (qtyInMainInvoice < item.qty)
                        {
                            ItemsNotAvailable.Add(item.ItemId);
                            if (item.ItemTypeId == (int)ItemTypes.Expiary && item.ExpireDate != null)
                                expiaryItemsNotAvailable.Add(item.ExpireDate.Value.Date.ToShortDateString());
                        }

                    }

                }
            }
               var ItemsNotAvailableName = itemCardMasterRepository.TableNoTracking.Where(a => ItemsNotAvailable.Contains(a.Id)).Select(a => a.ArabicName).ToList();

            var msg = getMSGforcheckQuantityBeforeSave(ItemsNotAvailableName, expiaryItemsNotAvailable, storeHasQty);

            return new Tuple<bool, string, string>((ItemsNotAvailable.Count() > 0 || !storeHasQty ? false : true), msg.Item1, msg.Item2);

        }
        public Tuple<string, string> getMSGforcheckQuantityBeforeSave(List<string> ItemsNotAvailable, List<string> expiaryItemsNotAvailable, bool storeHasQty)
        {
            var msgAr = "";
            var msgEn = "";
            if (ItemsNotAvailable.Count() > 0)
            {

                var ItemsNotAvailable_ = string.Join(" , ", ItemsNotAvailable.ToArray());
                msgAr = ErrorMessagesAr.QuantityNotAvailableForItem + ItemsNotAvailable_;
                msgEn = ErrorMessagesEn.QuantityNotAvailableForItem + ItemsNotAvailable_;

            }

            if (expiaryItemsNotAvailable.Count() > 0)
            {
                var expiaryItemsNotAvailable_ = string.Join(" , ", expiaryItemsNotAvailable.ToArray());
                msgAr = msgAr + " لتواريخ الصلاحية " + expiaryItemsNotAvailable_;
                msgEn = msgEn + " for expiry dates " + expiaryItemsNotAvailable_;
            }
            //if (!storeHasQty)
            //{
            //    msgAr = ErrorMessagesAr.ThereIsNoQuantityInStore;
            //    msgEn = ErrorMessagesEn.ThereIsNoQuantityInStore;
            //}
            return new Tuple<string, string>(msgAr, msgEn);

        }

        public async Task<Tuple<bool, string, string>> checkQuantityBeforeSaveInvoiceForAdd(List<InvoiceDetailsRequest> invoiceDetails, int storeId, DateTime invoiceDate, int invoiceId, InvGeneralSettings? setting, int invoiceTypeId, int signal, int newStoreId, bool RejectedTransfer)
        {
            var itemsNotCheckedQty = new List<int> { (int)ItemTypes.Serial, (int)ItemTypes.Service, (int)ItemTypes.Note };

            //1
            var currentInvoice = invoiceDetails.Where(a => !itemsNotCheckedQty.Contains(a.ItemTypeId))
                .GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                .Select(a => new { a.Key.SizeId, a.First().ItemTypeId, a.First().ItemCode, a.Key.ItemId, qty = a.Sum(e => e.Quantity * e.ConversionFactor), a.Key.ExpireDate }).ToList();
            var ItemsNotAvailable = new List<int>();
            var expiaryItemsNotAvailable = new List<string>();
            bool NotCheckQuantity = true;
            bool storeHasQty = true;

            if (Lists.InvoicesTypeForExtractStore_Setting.Contains(invoiceTypeId))
                NotCheckQuantity = setting.Sales_ExtractWithoutQuantity;
            else if (Lists.InvoicesTypeForAddStore_Setting.Contains(invoiceTypeId))
                NotCheckQuantity = setting.Purchases_ReturnWithoutQuantity;
            else if (Lists.POSInvoicesList.Contains(invoiceTypeId))
                NotCheckQuantity = setting.Pos_ExtractWithoutQuantity;
            else if (Lists.transferStore.Contains(invoiceTypeId))
                NotCheckQuantity = false;

            bool hasExpirydates = false;
            if (NotCheckQuantity)
            {

                var ExpiaryInCurrentInvoice = currentInvoice.Where(a => a.ItemTypeId == (int)ItemTypes.Expiary).ToList();
                if (ExpiaryInCurrentInvoice.Count() > 0)
                {
                     currentInvoice = ExpiaryInCurrentInvoice;
                    hasExpirydates = true;
                }
            }
            if (!NotCheckQuantity || hasExpirydates)
            {
                var QuantityDto = new List<QuantityDto>();
                if (invoiceId > 0 && Lists.QuantityNotCheckedInvoicesList.Contains(invoiceTypeId))
                {
                    //2 
                    var mainInvoice = invoiceDetailsQuery.TableNoTracking
                        .Where(a => !itemsNotCheckedQty.Contains(a.ItemTypeId) && a.InvoiceId == invoiceId && a.InvoicesMaster.StoreId == storeId
                                && (NotCheckQuantity? a.ItemTypeId == (int)ItemTypes.Expiary : true)) //  بعمل استثناء للسيريال من التشيك لانه بيتم عمل تشيك ف فنكشن تانيه
                         .GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                    .Select(a => new { a.Key.SizeId, a.Key.ItemId, qty = a.Sum(e => e.Quantity * e.ConversionFactor), a.Key.ExpireDate }).ToList();

                    //3  DifQuantity
                    // quantity decreased from invoice
                    //3.1
                    foreach (var item in currentInvoice)
                    {
                        var qtyInMainInvoice = mainInvoice.Where(a => a.ItemId == item.ItemId && a.SizeId == item.SizeId &&
                                      (item.ItemTypeId != (int)ItemTypes.Expiary ? true : a.ExpireDate.Value.Date == item.ExpireDate.Value.Date)
                                      ).Select(a => a.qty).Sum();
                        if (item.qty < qtyInMainInvoice || storeId != newStoreId) // 
                        {
                            var Qty = new QuantityDto();
                            Qty.qty = (storeId != newStoreId) ? qtyInMainInvoice : qtyInMainInvoice - item.qty;
                            Qty.itemId = item.ItemId;
                            Qty.expiryDate = item.ExpireDate;
                            QuantityDto.Add(Qty);
                        }

                    }
                    // items removed from invoice
                    //3.2
                    var removedItems = mainInvoice.Where(a => !currentInvoice.Where(w => w.ItemId == a.ItemId && w.SizeId == a.SizeId &&
                                       w.ExpireDate == a.ExpireDate).Select(e => e.ItemId).Contains(a.ItemId));
                    if (removedItems.Count() > 0)
                    {
                        foreach (var item in removedItems)
                        {
                            var Qty = new QuantityDto();
                            Qty.qty = item.qty;
                            Qty.itemId = item.ItemId;
                            Qty.expiryDate = item.ExpireDate;
                            QuantityDto.Add(Qty);
                        }
                    }



                    //4
                    var qtyAvailableInStore = invoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster)
                        .Where(a => a.InvoicesMaster.StoreId == storeId && ((invoiceId > 0 && Lists.QuantityNotCheckedInvoicesList.Contains(invoiceTypeId)) ?
                             QuantityDto.Select(e => e.itemId).Contains(a.ItemId) : currentInvoice.Select(e => e.ItemId).Contains(a.ItemId)) &&
                              (a.ExpireDate != null ? a.ExpireDate > invoiceDate : true)).GroupBy(a => new { a.ItemId, a.SizeId, a.ExpireDate })
                           .Select(a =>
                            new
                            {
                                a.Key.SizeId,
                               // itemName = a.First().Items.ArabicName,
                                a.Key.ItemId,
                                qty = a.Sum(e => e.Quantity * e.ConversionFactor * e.Signal),
                                a.Key.ExpireDate
                            }).ToList();


                    foreach (var item in qtyAvailableInStore)
                    {
                        var qtyOfItemInStore = item.qty;
                        double qtyOfItemInCurrentInvOice = 0;
                        if (invoiceId > 0 && Lists.QuantityNotCheckedInvoicesList.Contains(invoiceTypeId))
                            qtyOfItemInCurrentInvOice = QuantityDto.Where(a => a.itemId == item.ItemId && (item.SizeId != null ? a.sizeId == item.SizeId : true) &&
                                     (item.ExpireDate != null ? a.expiryDate == item.ExpireDate : true)).Sum(e => e.qty);
                        else
                            qtyOfItemInCurrentInvOice = currentInvoice.Where(a => a.ItemId == item.ItemId &&
                                       (item.SizeId != null ? a.SizeId == item.SizeId : true) &&
                                       (item.ExpireDate != null ? a.ExpireDate == item.ExpireDate : true)).Sum(a => a.qty);

                        if (qtyOfItemInStore < qtyOfItemInCurrentInvOice)
                        {
                            ItemsNotAvailable.Add(item.ItemId);
                            if (item.ExpireDate != null)
                                expiaryItemsNotAvailable.Add(item.ExpireDate.Value.ToShortDateString());
                        }
                    }
                    //  }

                }
            }

            //5  check qty in invoice

            if (ItemsNotAvailable.Count() <= 0)
            {
                if (Lists.returnInvoiceList.Contains(invoiceTypeId) || (invoiceTypeId == (int)DocumentType.IncomingTransfer && !RejectedTransfer))
                {

                    var mainInvoice = invoiceDetailsQuery.TableNoTracking
                   .Where(a => a.InvoiceId == invoiceId)
               .Select(a => new
               {
                   a.SizeId,
                   a.ItemId,
                   qty = (a.Quantity * a.ConversionFactor) - (a.ReturnQuantity * a.ConversionFactor),
                   a.ExpireDate,
                   a.indexOfItem
               }).ToList();

                    var _currentInvoice = invoiceDetails.Where(a => !itemsNotCheckedQty.Contains(a.ItemTypeId))
            .Select(a => new
            {
                a.SizeId,
                a.ItemTypeId,
                a.ItemCode,
                a.ItemId,
                qty = (a.Quantity * a.ConversionFactor),
                a.ExpireDate,
                a.IndexOfItem
            }).ToList();
                    foreach (var item in _currentInvoice)
                    {
                        var qtyInMainInvoice = mainInvoice.Where(a => a.ItemId == item.ItemId &&  a.indexOfItem == item.IndexOfItem &&
                      ((a.SizeId == null || a.SizeId == 0) ? true : a.SizeId == item.SizeId) &&
                       (item.ItemTypeId != (int)ItemTypes.Expiary ? true : a.ExpireDate.Value.Date == item.ExpireDate.Value.Date)
                                ).Sum(e => e.qty);
                        if (qtyInMainInvoice < item.qty)
                        {
                            ItemsNotAvailable.Add(item.ItemId);
                            if (item.ItemTypeId == (int)ItemTypes.Expiary && item.ExpireDate != null)
                                expiaryItemsNotAvailable.Add(item.ExpireDate.Value.Date.ToShortDateString());
                        }

                    }
                }
            }
            var ItemsNotAvailableName = itemCardMasterRepository.TableNoTracking.Where(a => ItemsNotAvailable.Contains(a.Id)).Select(a => a.ArabicName).ToList();


            var msg = getMSGforcheckQuantityBeforeSave(ItemsNotAvailableName, expiaryItemsNotAvailable, storeHasQty);

            return new Tuple<bool, string, string>((ItemsNotAvailable.Count() > 0 ? false : true), msg.Item1, msg.Item2);

        }


        public async Task<ResponseResult> ValidationOfInvoices(InvoiceMasterRequest parameter, int invoiceTypeId, InvGeneralSettings setting, int currentBranchId, bool recjectedTransfer, int[] userStors , DateTime invoiceDate, bool isUpdate)
        {
            var maxItems = (int)generalEnum.maxItemsOfInvoice;
            if (Lists.POSInvoicesList.Contains(invoiceTypeId))
                maxItems = (int)generalEnum.maxItemsOfPOS;
            if (parameter.InvoiceDetails.Count() > maxItems)
            {
                var maxcount = (int)generalEnum.maxItemsOfInvoice;
                return new ResponseResult()
                {
                    Result = Result.MaxCountOfItems,
                    Note = Actions.MaxItemsCountOfInvoice + maxcount,
                    ErrorMessageAr = maxcount + ErrorMessagesAr.MaxItemsCountOfInvoice,
                    ErrorMessageEn = ErrorMessagesEn.MaxItemsCountOfInvoice + maxcount
                };
            }
            // store
            if (parameter.StoreId == null || parameter.StoreId == 0)
                return new ResponseResult { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.SelectStore, ErrorMessageEn = ErrorMessagesEn.SelectStore };
            else
            {
                var storeIsExist = storeQuery.TableNoTracking.Where(a => a.Id == parameter.StoreId);
                if (storeIsExist.Count() == 0)
                    return new ResponseResult { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.storeNotExist, ErrorMessageEn = ErrorMessagesEn.storeNotExist };

            }
            if (Lists.transferStore.Contains(invoiceTypeId))
            {
                if ((parameter.StoreIdTo == 0 || parameter.StoreIdTo == null) && !recjectedTransfer)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        ErrorMessageAr = ErrorMessagesAr.SelectStoreTo,
                        ErrorMessageEn = ErrorMessagesEn.SelectStoreTo
                    };
                }
                else
                {
                    var storeIsExist = storeQuery.TableNoTracking.Where(a => a.Id == parameter.StoreIdTo);
                    if (storeIsExist.Count() == 0)
                        return new ResponseResult { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.storeToNotExist, ErrorMessageEn = ErrorMessagesEn.storeToNotExist };

                }
                if (parameter.StoreId == parameter.StoreIdTo)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        ErrorMessageAr = ErrorMessagesAr.StoreFromNottheSameStoreTo,
                        ErrorMessageEn = ErrorMessagesEn.StoreFromNottheSameStoreTo
                    };
                }
            }
            // check permissions on store for user account
            if (!userStors.Contains(parameter.StoreId))
            {
                return new ResponseResult()
                {
                    ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermissionOnStore,
                    ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermissionOnStore,
                    Result = Result.Failed
                };
            }
            // in case of offer price or purchase order 
            if(parameter.OfferPriceId>0)
            {
                TransferToSalesRequest req = new TransferToSalesRequest();
                    req.Id = parameter.OfferPriceId;
                var res = await _mediator.Send(req);
                if (res.Result != Result.Success)
                    return res;
            }
            // check if store in curent branch 
            if (invoiceTypeId != (int)DocumentType.IncomingTransfer)
            {
                var currentBranch = invStoreBranchQuery.TableNoTracking
                    .Where(a => a.StoreId == (invoiceTypeId == (int)DocumentType.IncomingTransfer ? parameter.StoreIdTo : parameter.StoreId))
                    .Select(a=>a.BranchId);
                if (!currentBranch.Contains(  currentBranchId))
                    return new ResponseResult()
                    {
                        Note = Actions.StoreAndCurrentBranchDoseNotMatch,
                        ErrorMessageAr = ErrorMessagesAr.storeNotLinkToThisBranch,
                        ErrorMessageEn = ErrorMessagesEn.storeNotLinkToThisBranch,
                        Result = Result.Failed
                    };
            }

            DateTime date = DateTime.Parse("1/1/0001 12:00:00 AM"); //DateTime.ParseExact("1/1/0001 12:00:00 AM", "yyyy-MM-dd HH:mm tt");
            // invoice date 
            if (parameter.InvoiceDate == null || parameter.InvoiceDate == date)
                return new ResponseResult { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.EnterInvoiceDate, ErrorMessageEn = ErrorMessagesEn.EnterInvoiceDate };


            // person 
            if (Lists.purchasesInvoicesList.Contains(invoiceTypeId) || invoiceTypeId == (int)DocumentType.PurchaseOrder || Lists.salesInvoicesList.Contains(invoiceTypeId) 
                || Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId)
                    || Lists.POSInvoicesList.Contains(invoiceTypeId) || invoiceTypeId==(int)DocumentType.OfferPrice)
                if (parameter.PersonId != null)
                {
                    var rightPerson = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == parameter.PersonId.Value &&
                    a.PersonBranch.Where(e => e.BranchId == currentBranchId && e.PersonId == parameter.PersonId.Value).Any() &&
                      (Lists.purchasesInvoicesList.Contains(invoiceTypeId) || Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId) ? a.IsSupplier == true : a.IsCustomer == true));
                    if (rightPerson.Count() == 0)
                        return new ResponseResult()
                        {
                            Data = null,
                            Id = null,
                            Result = Result.NotExist,
                            ErrorMessageAr = (Lists.purchasesInvoicesList.Contains(invoiceTypeId) ? ErrorMessagesAr.SupplierNotExist : ErrorMessagesAr.CustomerNotExist)
                           ,
                            ErrorMessageEn = (Lists.purchasesInvoicesList.Contains(invoiceTypeId) ? ErrorMessagesEn.SupplierNotExist : ErrorMessagesEn.CustomerNotExist)
                        };

                }

            // salsMan
   
            if (Lists.salesInvoicesList.Contains(invoiceTypeId) || invoiceTypeId == (int)DocumentType.OfferPrice)
            {
                if (parameter.SalesManId == null || parameter.SalesManId == 0)
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.RequiredData,
                        Note = "you should select salesman"
                        ,
                        ErrorMessageAr = ErrorMessagesAr.SelectSalesMan,
                        ErrorMessageEn = ErrorMessagesEn.SelectSalesMan
                    };
                if (setting.Sales_LinkRepresentCustomer && !isUpdate)
                {
                    var isSameSalesMan = PersonRepositorQuery.TableNoTracking.Where(a => a.Id == parameter.PersonId && a.SalesManId == parameter.SalesManId);
                    if (isSameSalesMan == null || isSameSalesMan.Count() == 0)
                    {
                        return new ResponseResult { Result = Result.Failed, ErrorMessageAr = ErrorMessagesAr.PaymentMethodvalueExceededCustomerBalance ,ErrorMessageEn=ErrorMessagesEn.PaymentMethodvalueExceededCustomerBalance };
                    }
                }

            }

        

            // details
            if (parameter.InvoiceDetails == null || parameter.InvoiceDetails.Count() == 0)
                return new ResponseResult()
                {
                    Data = null,
                    Id = null,
                    Result = Result.RequiredData,
                    Note = "you should send at least one item",
                    ErrorMessageAr = ErrorMessagesAr.NoItems,
                    ErrorMessageEn = ErrorMessagesEn.NoItems
                };



            // return
            // امنع ارجع الفاتوره المرتجعه من قبل والفاتوره الاصليه تبعها
            if (Lists.returnInvoiceList.Contains(invoiceTypeId) && !string.IsNullOrEmpty(parameter.ParentInvoiceCode))
            {

                var returnedBefore = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => (a.InvoiceSubTypesId == (int)SubType.TotalReturn || a.IsDeleted) &&
                                   (a.ParentInvoiceCode == parameter.ParentInvoiceCode || a.InvoiceType == parameter.ParentInvoiceCode)).ToList();
                if (returnedBefore.Count() > 0)
                {
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.InvoiceDeletedOrReturned,  //Note = "Invoice returned before",
                        ErrorMessageAr = (returnedBefore.First().IsDeleted ? ErrorMessagesAr.InvoiceNotExist : ErrorMessagesAr.InvoiceReturnedBefore)
                        ,
                        ErrorMessageEn = (returnedBefore.First().IsDeleted ? ErrorMessagesEn.InvoiceNotExist : ErrorMessagesEn.InvoiceReturnedBefore)
                    };
                }

                var mainInvoice = InvoiceMasterRepositoryQuery.TableNoTracking
                                .Where(a => a.InvoiceType == parameter.ParentInvoiceCode.Trim() || a.Code.ToString() == parameter.ParentInvoiceCode.Trim())
                                .Select(a => new { a.InvoiceId, a.StoreId, a.PersonId, a.InvoicesDetails, a.SalesManId }).ToList();
                if (mainInvoice.Count() == 0)
                    return new ResponseResult() { Result = Result.NotExist, Note = "Invoice does not exist", ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist, ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
                if (mainInvoice.First().StoreId != parameter.StoreId)
                    return new ResponseResult() { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.SelectSameStoreOfMainInvoice, ErrorMessageEn = ErrorMessagesEn.SelectSameStoreOfMainInvoice };
                if (mainInvoice.First().PersonId != parameter.PersonId)
                    return new ResponseResult() { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.SelectSamePersonOfMainInvoice, ErrorMessageEn = ErrorMessagesEn.SelectSamePersonOfMainInvoice };
                if (invoiceTypeId == (int)DocumentType.ReturnSales && mainInvoice.First().SalesManId != parameter.SalesManId)
                    return new ResponseResult() { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.SelectSamePersonOfMainInvoice, ErrorMessageEn = ErrorMessagesEn.SelectSamePersonOfMainInvoice };

                var itemsOfInvoice = mainInvoice.FirstOrDefault().InvoicesDetails.Select(a => a.ItemId);
                var itemsInRequest = parameter.InvoiceDetails.Select(a => a.ItemId);
                var expectItems = itemsInRequest.Except(itemsOfInvoice).ToList();
                if (expectItems.Any())
                    return new ResponseResult()
                    {
                        Data = expectItems.ToArray(),
                        Result = Result.NotExist,
                        ErrorMessageAr = ErrorMessagesAr.ItemNotExist + " " + String.Join(",", expectItems),
                        ErrorMessageEn = ErrorMessagesEn.ItemNotExist + " " + String.Join(",", expectItems)
                    };


            }
            if (parameter.Paid == null)
                parameter.Paid = 0;

            if(Lists.POSInvoicesList.Contains(invoiceTypeId))
            {
                if (!setting.Pos_EditingOnDate && parameter.InvoiceDate.ToShortDateString() != invoiceDate.ToShortDateString())
                {
                    return new ResponseResult()
                    {  Result = Result.editingDate, Note = "Editing invoice date is not permitted",
                        ErrorMessageAr = ErrorMessagesAr.editingDate, ErrorMessageEn = ErrorMessagesEn.editingDate
                    };
                }
            }

            
           /* if(invoiceTypeId == (int)DocumentType.Sales || invoiceTypeId == (int)DocumentType.Purchase)
            {

                var value = parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).Select(a => a.Value);
                if (value != null && value.Count() > 0)
                {
                    var request = new GetPersonBalanceForPaymentMethodRequest() { invoiceTypeId = invoiceTypeId, personId = parameter.PersonId.Value,BranchId= currentBranchId };
                    var result = await _mediator.Send(request);
                    if (value.First() > result.balance)
                    {
                        if(invoiceTypeId == (int)DocumentType.Sales)
                        {
                            return new ResponseResult()
                            {
                                Result = Result.ExceedPersonBalance, Note = "Exceeded person balance",
                                ErrorMessageAr = ErrorMessagesAr.PaymentMethodvalueExceededCustomerBalance,
                                ErrorMessageEn = ErrorMessagesEn.PaymentMethodvalueExceededCustomerBalance
                            };
                        }
                        else if(invoiceTypeId == (int)DocumentType.Purchase)
                        {
                            return new ResponseResult()
                            {
                                Result = Result.ExceedPersonBalance, Note = "Exceeded person balance",
                                ErrorMessageAr = ErrorMessagesAr.PaymentMethodvalueExceededSupplierBalance,
                                ErrorMessageEn = ErrorMessagesEn.PaymentMethodvalueExceededSupplierBalance
                            };
                        }
                    }
                 

                }
            }*/
            return new ResponseResult() { Result = Result.Success };
        }
        
        public async Task<ResponseResult> GetSystemHistoryLogs(int pageNumber, int pageSize)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var History = _systemHistoryLogsQuery.TableNoTracking
                .Include(x => x.employees)
                .Where(x => !userInfo.otherSettings.showDashboardForAllUsers && userInfo.userId != 1 ? x.employeesId == userInfo.employeeId : true)
                .Where(x => x.TransactionId != (int)SystemActionEnum.login ? x.BranchId == userInfo.CurrentbranchId : true)
                .OrderByDescending(x => x.Id).Take(100)
                .Select(x => new HistoryMovement
                {
                    ArabicName = x.employees.ArabicName,
                    LatinName = x.employees.LatinName,
                    DateTime = x.date,
                    ArabicTransactionType = x.ActionArabicName,
                    LatinTransactionType = x.ActionLatinName
                });
            History = History.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new ResponseResult()
            {
                Data = History,
                Result = Result.Success,
                DataCount = History.Count(),
                Note = pageNumber >= 5 ? "End Of Data" : ""
            };
        }

        public async Task<ResponseResult> SendingEmail(emailRequest parm)
        {
            if (!Regex.IsMatch(parm.ToEmail, "^\\S+@\\S+\\.\\S+$"))
                return new ResponseResult()
                {
                    ErrorMessageAr = "البريد الالكتروني غير صحيح",
                    ErrorMessageEn = "Email is not correct",
                    Result = Result.Failed
                };

            var sendEmail = await _emailService.SendEmail(parm);

            return new ResponseResult()
            {
                Note = sendEmail == "OK" ? Actions.Success : Actions.SaveFailed,
                Result = sendEmail == "OK" ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> getPersonEmail(int personId)
        {
            var person = await PersonRepositorQuery.GetByIdAsync(personId);
            if (person == null)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            return new ResponseResult()
            {
                Data = person.Email,
                Note = Actions.Success,
                Result = Result.Success
            };
        }
        #endregion

        public DateTime serverDate(DateTime Dt)
        {
            string time = DateTime.Now.ToString("HH:mm:ss tt");
            DateTime dt = Convert.ToDateTime(Dt.ToShortDateString() + " " + time);
            return dt;
        }

        public async Task<ResponseResult> GetItemsDropDownForReports(dropdownListOfItemsForReports parm)
        {

            var items = itemCardMasterRepository.TableNoTracking.Include(x => x.Units).ToList();
            var units = _invStpUnitsQuery.TableNoTracking.Select(x => new
            {
                x.Id,
                x.ArabicName,
                x.LatinName
            });
            if (!string.IsNullOrEmpty(parm.name))
                items = items.Where(x => x.ArabicName.Contains(parm.name)).ToList();
            if (!string.IsNullOrEmpty(parm.code))
                items = items.Where(x => x.ItemCode == parm.code || x.NationalBarcode == parm.code || x.Units.Select(c=> c.Barcode).Contains(parm.code)).ToList();
            items = items.Skip((parm.pageNumber - 1) * parm.pageSize).Take(parm.pageSize).ToList();
            var res = items.Select(x => new
            {
                x.Id,
                x.ArabicName,
                x.LatinName,
                x.ItemCode,
                units = units.Where(c => x.Units.Select(t => t.UnitId).Contains(c.Id)).Select(d => new
                {
                    d.Id,
                    d.ArabicName,
                    d.LatinName,
                    isDefult = x.ReportUnit == d.Id ? true : false
                })
            });
            double MaxPageNumber = items.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                Note = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                TotalCount = items.Count()
            };
        }

        public async Task<ResponseResult> GetStoresDropDownForReports(dropdownListOfItemsForReports parm)
        {
            var userInfo = await Userinformation.GetUserInformation();
            var stores = storeQuery.TableNoTracking.Include(a => a.StoreBranches)
                        .Where(x => userInfo.employeeBranches.Contains(x.StoreBranches.First().BranchId)).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        });
            var res = stores.Skip((parm.pageNumber - 1) * parm.pageSize)
                                .Take(parm.pageSize)
                                .ToList();
            double MaxPageNumber = stores.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                Note = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                TotalCount = stores.Count()
            };


        }

        public async Task<int> AddPrintFiles()
        {
            try
            {
                var allFileNames = GetListOfFile.ReportFilesList();
                var fileNamesFromDb = await _iReportFileService.GetAllReportFiles();
                var fileNamesToAdd = new List<ReportFiles>();
                if (allFileNames.Count > 0 && fileNamesFromDb.Count() > 0)
                {
                    foreach (var item in fileNamesFromDb)
                    {

                        foreach (var file in allFileNames)
                        {

                            if (item.ReportFileName == file.reportName)
                            {
                                allFileNames.Remove(file);
                                break;
                            }


                        }
                    }
                }
                if (allFileNames.Count > 0)
                {
                    foreach (var file in allFileNames)
                    {

                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = true,
                            IsReport = 0,
                            ReportFileName = file.reportName,
                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, true)

                        });
                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = false,
                            IsReport = 0,
                            ReportFileName = file.reportName,
                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, false)

                        });

                    }

                    _reportFileCommand.AddRangeAsync(fileNamesToAdd);
                    await _reportFileCommand.SaveChanges();
                    var ReportFileManagerToAdd = new List<ReportManger>();
                    foreach (var item in fileNamesToAdd)
                    {
                        ReportFileManagerToAdd.Add(new ReportManger
                        {
                            IsArabic = item.IsArabic,
                            Copies = 1,
                            ArabicFilenameId = item.Id,
                            screenId = allFileNames.Where(x => x.reportName == item.ReportFileName).FirstOrDefault().screenId
                        });
                    }

                    _reportManagerCommand.AddRangeAsync(ReportFileManagerToAdd);
                    _reportManagerCommand.SaveChanges();

                }
                return Defults.updateNumber;

            }
            catch (Exception)
            {
                return 0;
            }



        }
        public async Task<int> UpdatePrintFiles()
        {
            try
            {
                var fileNamesFromDb = await _iReportFileService.GetAllReportFiles();
                var arFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\ar");
                var arfiles = from file in
                Directory.EnumerateFiles(arFilesPath)
                              select file;

                var enFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\en");
                var enfiles = from file in
                Directory.EnumerateFiles(enFilesPath)
                              select file;
                string fileName = "";
                foreach (var item in fileNamesFromDb)
                {
                    if (item.IsArabic == true)
                    {
                        foreach (var file in arfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);
                            if (item.ReportFileName == fileName)
                            {
                                // var fileName = Path.GetFileName(file); 
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                break;
                            }
                        }
                    }

                    else
                    {
                        foreach (var file in enfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);

                            if (item.ReportFileName == fileName)
                            {
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                break;
                            }
                        }
                    }
                }
                await _reportFileCommand.UpdateAsyn(fileNamesFromDb);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

            // Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, true)

        }

        public bool CompareSerialsWithMainInvoice(List<InvoiceDetailsRequest> requestInvoiceDetails, string parentInvoiceType, int invoiceTypeId)
        {

            var serialsListMainInvoice = serialTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == parentInvoiceType
                 || a.ExtractInvoice == parentInvoiceType).Select(a => a.SerialNumber).ToList();

            var serialsListRequest = new List<string>();
            requestInvoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).Select(a => a.ListSerials).ToList()
            .ForEach(a => serialsListRequest.AddRange(a));
            serialsListRequest = serialsListRequest.ConvertAll(d => d.ToUpper());

            //if(invoiceTypeId==(int)DocumentType.IncomingTransfer)
            //{
            //    //if (serialsListMainInvoice.Count() != serialsListRequest.Count())
            //    //    return false;
            //    var diffSerials1 = serialsListMainInvoice.Except(serialsListRequest);
            //    var diffSerials2 = serialsListRequest.Except(serialsListMainInvoice);
            //    return (diffSerials1.Count() <= 0 && diffSerials2.Count() <= 0);

            //}
            //else
            //{
            var serialExist = serialsListRequest.Where(a => serialsListMainInvoice.Contains(a));
            if (serialExist.Count() != serialsListRequest.Count())
                return false;
            //     }
            return true;
        }
        public TransferDesc GetTransferDesc(int invoicesTypeID, int storeFrom, int? storeto, bool isRejected)
        {
            var store = storeQuery.TableNoTracking.Where(h => h.Id == storeFrom || h.Id == storeto).ToList();
            string descAr = "";
            string descEn = "";
            var storeFromAr = store.Where(a => a.Id == storeFrom).FirstOrDefault()?.ArabicName ?? "";
            var storeFromEn = store.Where(a => a.Id == storeFrom).FirstOrDefault()?.LatinName ?? "";
            var storeToAr = store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? "";
            var storeToEn = store.Where(a => a.Id == storeto).FirstOrDefault()?.LatinName ?? "";
            if (invoicesTypeID == (int)DocumentType.IncomingTransfer)
            {
                if (isRejected)
                {
                    //descAr = String.Concat(" تحويل مرفوض من ", storeFromAr, " الى ", storeToAr);
                    //descEn = String.Concat(" Rejected Transfer From ", storeFromEn, " to ", storeToEn);
                    descAr = $"تحويل مرفوض من {store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? ""}";
                    descEn = $"Rejected Transfer From {store.Where(a => a.Id == storeto).FirstOrDefault()?.LatinName ?? ""}";
                }
                else
                {
                    //descAr = String.Concat(" تحويل وارد من ", storeFromAr, " الى ", storeToAr);
                    //descEn = String.Concat(" Incoming Transfer From ", storeFromEn, " to ", storeToEn);

                    descAr = $"تحويل وارد من {store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? ""}";
                    descEn = $"Incoming Transfer From {store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? ""}";
                }
            }
            else if (invoicesTypeID == (int)DocumentType.OutgoingTransfer)
            {
                //descAr = String.Concat(" تحويل صادر من ", storeFromAr, " الى ", storeToAr);
                //descEn = String.Concat(" Outgoing Transfer From ", storeFromEn, " to ", storeToEn);

                descAr = $"تحويل صادر الي مخزن   {store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? ""}";
                descEn = $"Outgoing Transfer To {store.Where(a => a.Id == storeto).FirstOrDefault()?.ArabicName ?? ""}";
            }
            return new TransferDesc()
            {
                descAr = descAr,
                descEn = descEn
            };
        }
        public Tuple<bool, ResponseResult> userHasSession(int? sessionId)
        {
            if (sessionId == null || sessionId == 0)
                return new Tuple<bool, ResponseResult>(false, new ResponseResult()
                { Result = Result.UnAuthorized, ErrorMessageAr = ErrorMessagesAr.UnauthorizedOnSession ,ErrorMessageEn=ErrorMessagesEn.UnauthorizedOnSession}) ;
         
            return new Tuple<bool, ResponseResult>(true , null);
        }
        public async Task<ResponseResult> Pagination<T>(List<T> resData , int pageNumber , int pageSize )
        {
            var count = resData.Count();


            if (pageSize > 0 && pageNumber > 0)
            {
                resData = resData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }
            return new ResponseResult() { Data = resData, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };
        }

    }
    public class QuantityInStoreAndInvoice
    {
        public double InvoiceQuantity { get; set; } // الكمية فى الفاتورة
        public double StoreQuantity { get; set; }  // الكميه الفعليه ف المخزن
        public double StoreQuantityWithOutInvoice { get; set; } // الكمية فى المخزن باستثناء الفاتورة   

        // public Result result { get; set; }

    }



    // return parameter of CheckQuantityBeforeSave 
    public class checkQuantityBeforeSaveingInvoice
    {
        public InvoiceMasterRequest Invoice { get; set; }// if quantity not available , get available quantity in invoice and reset the list
        public bool QuantityAvailable { get; set; }
    }

}
