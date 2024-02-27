using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Infrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Net.Http.Headers;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public class DeleteInvoice : BaseClass, IDeleteInvoice
    {
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
         private readonly IRepositoryQuery<InvoicePaymentsMethods> PaymentsMethodsRepositoryQuery;

        private readonly IRepositoryCommand<InvoicePaymentsMethods> PaymentsMethodsRepositoryCommand;
        private readonly IRepositoryCommand<InvPurchaseAdditionalCostsRelation> InvoiceAdditionalCostsRelationRepositoryCommand;
        private readonly IRepositoryQuery<InvPurchaseAdditionalCostsRelation> AdditionalCostsRelationRepositoryQuery;

        private readonly IRepositoryCommand<InvoiceDetails> InvoiceDetailsRepositoryCommand;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;

         private readonly IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery;

        private readonly IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterRepositoryQuery;

        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;
        private readonly ISerialsService serialsService;
        private readonly IReceiptsFromInvoices ReceiptsInvoice;

        // private SettingsOfInvoice SettingsOfInvoice;
        private readonly IHttpContextAccessor httpContext;
        public DeleteInvoice(
                              IRepositoryCommand<InvoiceMaster> _InvoiceMasterRepositoryCommand,
                              IRepositoryCommand<InvoiceDetails> _InvoiceDetailsRepositoryCommand,
                              IRepositoryQuery<InvoiceDetails> _InvoiceDetailsRepositoryQuery,
                               IRepositoryCommand<InvoicePaymentsMethods> _PurchasePaymentsMethodsRepositoryCommand,
                              IRepositoryQuery<InvoicePaymentsMethods> _PaymentsMethodsRepositoryQuery,
                              IRepositoryCommand<InvPurchaseAdditionalCostsRelation> _PurchaseAdditionalCostsRelationRepositoryCommand,
                              IRepositoryQuery<InvPurchaseAdditionalCostsRelation> _AdditionalCostsRelationRepositoryQuery,
                               IRepositoryQuery<InvStpItemCardMaster> _InvStpItemCardMasterRepositoryQuery,
                              IHistoryInvoiceService _HistoryInvoiceService,
                              IGeneralAPIsService _GeneralAPIsService,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              iInvoicesIntegrationService iInvoicesIntegrationService,
                               IRepositoryQuery<InvSerialTransaction> _InvSerialTransactionRepositoryQuery,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              ISerialsService serialsService, IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery,
                              IHttpContextAccessor _httpContext, IReceiptsFromInvoices receiptsInvoice) : base(_httpContext)
        {
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            InvoiceDetailsRepositoryCommand = _InvoiceDetailsRepositoryCommand;
            InvoiceDetailsRepositoryQuery = _InvoiceDetailsRepositoryQuery;
            PaymentsMethodsRepositoryCommand = _PurchasePaymentsMethodsRepositoryCommand;
             InvStpItemCardMasterRepositoryQuery = _InvStpItemCardMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            AdditionalCostsRelationRepositoryQuery = _AdditionalCostsRelationRepositoryQuery;
            PaymentsMethodsRepositoryQuery = _PaymentsMethodsRepositoryQuery;
            InvoiceAdditionalCostsRelationRepositoryCommand = _PurchaseAdditionalCostsRelationRepositoryCommand;
            httpContext = _httpContext;
             HistoryInvoiceService = _HistoryInvoiceService;
            InvSerialTransactionRepositoryQuery = _InvSerialTransactionRepositoryQuery;
            GeneralAPIsService = _GeneralAPIsService;
            this.systemHistoryLogsService = systemHistoryLogsService;
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
             this.serialsService = serialsService;
            this.InvoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            ReceiptsInvoice = receiptsInvoice;
        }
        //public async Task<ResponseResult> DeleteInvoices(int Ids, int invoiceType, string InvoiceTypeName)
        //{
        //    return await DeleteInvoices(Ids, invoiceType, InvoiceTypeName, invoiceType);
        //}
        public async Task<ResponseResult> DeleteInvoices(int Ids, int invoiceTypeId, string InvoiceTypeName, int MaininvoiceTytpe)
        {

           
            var reasonOfNotDeletedAr = "";
            var reasonOfNotDeletedEn = "";

            var invoicesListWOtype = InvoiceMasterRepositoryQuery.TableNoTracking.Include(a => a.InvoicesDetails)//.ThenInclude(a => a.Items.Serials)
                       .Where(e => Ids == e.InvoiceId).ToList();

            var invoicesList = invoicesListWOtype.Where(h => h.InvoiceTypeId == invoiceTypeId || h.InvoiceTypeId == MaininvoiceTytpe);
            if (invoicesListWOtype.Any() && invoicesList.Count() <= 0)
            {
              
                return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = " لا يمكن الحذف المستند نظرا لاختلاف ال API ", ErrorMessageEn = "you can not delete this rec as  Api is different " } ;
            }
            var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);
           
            if (Lists.MainInvoiceForReturn.Contains(MaininvoiceTytpe) && setting.ActiveElectronicInvoice)
            {
                return new ResponseResult { Result = Result.CanNotBeUpdated, ErrorMessageAr = ErrorMessagesAr.CantDeleteSystemInElectronicInvoice, ErrorMessageEn = ErrorMessagesEn.CantDeleteSystemInElectronicInvoice };
            }

            // invoice ids from request
            var idS = invoicesList.Select(a => a.InvoiceId).ToList();
            // invoices that deleted before or returned or accredited > prevent deleting them again
            //var invoiceCantDeleted_ = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => idS.Contains(a.InvoiceId) && a.InvoiceTypeId == MaininvoiceTytpe &&
            //           (a.IsDeleted || a.IsAccredite || a.IsReturn)).Select(a => new { a.InvoiceId, a.IsDeleted, a.IsAccredite, a.IsReturn, a.transferStatus }).ToList();
            if (invoicesList.Count() > 0)
            {
                if (invoicesList.First().IsAccredite)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantDeletedInvAccredited;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantDeletedInvAccredited;
                }
                else if (invoicesList.First().IsDeleted)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantDeletedInvDeleted;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantDeletedInvDeleted;
                }
                else if (invoicesList.First().IsReturn)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantDeletedInvReturned;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantDeletedInvReturned;
                }

                else if (invoicesList.First().transferStatus != Aliases.TransferStatus.Binded && Lists.transferStore.Contains(invoicesList.First().InvoiceTypeId))
                {
                    return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = "لا يمكن الحذف المستند", ErrorMessageEn = "you can not delete this rec " };
                }
                else if (invoicesList.First().IsCollectionReceipt)
                {
                    reasonOfNotDeletedAr = ErrorMessagesAr.CantDeleteInvCollectionReceipt;
                    reasonOfNotDeletedEn = ErrorMessagesEn.CantDeleteInvCollectionReceipt;
                }
                //التحويل الصادر فى حاله الاستلام 
                if (!string.IsNullOrEmpty(reasonOfNotDeletedAr))
                    return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = reasonOfNotDeletedAr, ErrorMessageEn = reasonOfNotDeletedEn };
            }
            List<int> invoiceCantDeleted = new List<int>();
            // check quantity prevent deleting if not available
            foreach (var item in invoicesList)
            {
                try
                {
                    
                    var signal = GeneralAPIsService.GetSignal(invoiceTypeId);

                    List<InvoiceDetailsRequest> invoiceDetailsReq = new List<InvoiceDetailsRequest>();
                    Mapping.Mapper.Map<List<InvoiceDetails>, List<InvoiceDetailsRequest>>(item.InvoicesDetails.ToList(), invoiceDetailsReq);


                    if (Lists.InvoicesTypesOfExtractFromStore.Contains(invoiceTypeId))
                    {
                        var quantityAvailable = GeneralAPIsService.checkQuantityBeforeSaveInvoiceForExtract(invoiceDetailsReq, item.StoreId, item.InvoiceDate, item.InvoiceId, setting, invoiceTypeId, signal, item.StoreId, true, InvoiceTypeName).Result;
                        if (!quantityAvailable.Item1)
                        {
                            reasonOfNotDeletedAr = ErrorMessagesAr.ThereIsNoQuantityInStore;
                            reasonOfNotDeletedEn = ErrorMessagesEn.ThereIsNoQuantityInStore;
                            invoiceCantDeleted.Add(item.InvoiceId);
                        }

                    }
                    else if (Lists.InvoicesTypeOfAddingToStore.Contains(invoiceTypeId))
                    {
                        var quantityAvailable = GeneralAPIsService.checkQuantityBeforeSaveInvoiceForAdd(invoiceDetailsReq, item.StoreId, item.InvoiceDate, item.InvoiceId, setting, invoiceTypeId, signal, item.StoreId,false).Result;
                        if (!quantityAvailable.Item1)
                        {
                            reasonOfNotDeletedAr = ErrorMessagesAr.ThereIsNoQuantityInStore;
                            reasonOfNotDeletedEn = ErrorMessagesEn.ThereIsNoQuantityInStore;
                            invoiceCantDeleted.Add(item.InvoiceId);
                        }
                    }
                    // get el serials
                    List<string> serialsList = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a =>( a.AddedInvoice == item.InvoiceType
                    || a.ExtractInvoice == item.InvoiceType) && !a.IsDeleted).Select(a => a.SerialNumber).ToList();

                    var serialsFromDb = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a => serialsList.Contains(a.SerialNumber)
              && (invoiceTypeId == (int)DocumentType.DeletedOutgoingTransfer ? a.TransferStatus == TransferStatus.Binded : true) &&
              a.ExtractInvoice == null && a.StoreId == invoicesList.First().StoreId)
              //(Lists.InvoicesTypesOfExtractFromStore.Contains(invoiceType) ? (a.ExtractInvoice == null) : true))//&& a.IsDeleted == false) // شيلت الكومنت عشان كيس حذف سيريال عند تعديل فاتورة الاضافة
                       .Select(a => a.SerialNumber);  // عملت كومنت عشان لو حذفت مبيعات ودخلت السيريال تانى فى المخزن مفروض ميحذفش المبيعات
                                                      //&& a.AddedInvoice == item.InvoiceType
                
               //       if (serialsFromDb.Count() != serialsList.Count())
                   if ((Lists.InvoicesTypeOfAddingToStore.Contains(invoiceTypeId) && serialsFromDb.Count() > 0) ||
             (Lists.InvoicesTypesOfExtractFromStore.Contains(invoiceTypeId) && serialsFromDb.Count() != serialsList.Count()))
                    { 
                        reasonOfNotDeletedAr = ErrorMessagesAr.ThereIsNoQuantityInStore;
                        reasonOfNotDeletedEn = ErrorMessagesEn.ThereIsNoQuantityInStore;
                        invoiceCantDeleted.Add(item.InvoiceId);
                    }

                }
                catch (Exception e)
                {

                    throw;
                }

            }


            // update old invoice , change isdelete = true
            var invoiceWillDelete = invoicesList.Where(a => !invoiceCantDeleted.Contains(a.InvoiceId)).ToList();
            if (invoiceWillDelete.Count() == 0)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageEn = (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedEn : ErrorMessagesEn.CanNotDeleteSomeInvoices),
                    ErrorMessageAr = (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedAr : ErrorMessagesAr.CanNotDeleteSomeInvoices)
                };

            invoiceWillDelete.Select(e => { e.IsDeleted = true;
                                     e.transferStatus = (e.InvoiceTypeId == (int)DocumentType.OutgoingTransfer ? TransferStatus.Deleted : 0);
                                   return e; }).ToList();
            
            try
            {
                var res = await InvoiceMasterRepositoryCommand.UpdateAsyn(invoiceWillDelete);

            }
            catch (Exception e)
            {

                throw;
            }
            try
            {
                foreach (var item in invoiceWillDelete)
                {

                    InvoiceMasterRepositoryCommand.StartTransaction();

                    item.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                    // validation of total result
                    var invoice = new InvoiceMaster()
                    {
                        BranchId = item.BranchId,
                        Code = item.Code,
                        InvoiceType = item.BranchId + "-" + InvoiceTypeName + "-" + item.Code,
                        InvoiceTypeId = invoiceTypeId,
                        BookIndex = item.BookIndex.Trim(),
                        InvoiceDate = item.InvoiceDate,
                        //InvoiceId=item.InvoiceId,
                        StoreId = item.StoreId,
                        PersonId = item.PersonId,
                        Notes = item.Notes.Trim(),
                        DiscountType = item.DiscountType,
                        Net = item.Net,
                        ActiveDiscount = item.ActiveDiscount,
                        BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                        Paid = item.Paid,
                        VirualPaid = item.VirualPaid,
                        ApplyVat = item.ApplyVat,
                        TotalAfterDiscount = item.TotalAfterDiscount,
                        InvoiceSubTypesId = item.InvoiceSubTypesId,
                        PaymentType = item.PaymentType,
                        ParentInvoiceCode = item.InvoiceType,
                        PriceWithVat = item.PriceWithVat,
                        Remain = item.Remain,
                        TotalDiscountRatio = item.TotalDiscountRatio,
                        TotalDiscountValue = item.TotalDiscountValue,
                        TotalPrice = item.TotalPrice,
                        TotalVat = item.TotalVat,
                        IsDeleted = true,
                        SalesManId = item.SalesManId,
                        StoreIdTo=item.StoreIdTo,
                        transferStatus= item.transferStatus,
                        

                    };
                    invoice.InvoiceTransferType = invoice.InvoiceType;
                    //purchase.ParentInvoiceCode = item.ParentInvoiceCode;
                    invoice.Serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(invoiceTypeId, item.InvoiceId, invoice.BranchId).ToString());

                    if (!Lists.storesInvoicesList.Contains(invoiceTypeId))
                    {
                        // determine type of payment
                        if (invoice.Paid == 0)
                        {
                            invoice.PaymentType = (int)PaymentType.Delay;
                        }
                        if (invoice.Paid < invoice.Net && invoice.Paid != 0)
                        {
                            invoice.PaymentType = (int)PaymentType.Partial;
                        }
                        if (invoice.Paid == invoice.Net)
                        {
                            invoice.PaymentType = (int)PaymentType.Complete;
                        }
                    }


                    var saved = await InvoiceMasterRepositoryCommand.AddAsync(invoice);
                    //   var saved=  await InvoiceMasterRepositoryCommand.SaveAsync();
                    if (!saved)
                        return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" + invoice.InvoiceType };

                    var details = InvoiceDetailsRepositoryQuery.FindAll(q => q.InvoiceId == item.InvoiceId);
                    // save details of items in the invoice
                    var invoiceDetailsList = new List<InvoiceDetails>();
                    var hasSerials = false;
                    foreach (var itemDetail in details)
                    {
                        var invoiceDetails = new InvoiceDetails();
                        invoiceDetails.InvoiceId = invoice.InvoiceId;
                        invoiceDetails.ItemId = itemDetail.ItemId;
                        invoiceDetails.Price = itemDetail.Price;
                        invoiceDetails.Quantity = itemDetail.Quantity;
                        invoiceDetails.TotalWithSplitedDiscount = itemDetail.TotalWithSplitedDiscount;// invoiceDetails.PurchasePrice * invoiceDetails.Quantity;
                        invoiceDetails.UnitId = itemDetail.UnitId;
                        invoiceDetails.ItemTypeId = itemDetail.ItemTypeId;
                        invoiceDetails.Signal = GeneralAPIsService.GetSignal(invoice.InvoiceTypeId);
                        invoiceDetails.ConversionFactor = itemDetail.ConversionFactor;
                        invoiceDetails.VatRatio = itemDetail.VatRatio;
                        invoiceDetails.VatValue = itemDetail.VatValue;
                        invoiceDetails.indexOfItem = itemDetail.indexOfItem;
                        var items = await InvStpItemCardMasterRepositoryQuery.GetByAsync(q => q.Id == invoiceDetails.ItemId);
                        if (items != null)
                        {
                            if (items.TypeId == (int)ItemTypes.Expiary)
                            {
                                invoiceDetails.ExpireDate = itemDetail.ExpireDate;
                            }
                            if (items.TypeId == (int)ItemTypes.Serial)
                            {
                                hasSerials = true;

                            }
                        }
                        invoiceDetailsList.Add(invoiceDetails);

                        // invoice.TotalPrice += invoiceDetails.Total;
                    }
                    if (hasSerials)
                    {
                        if (Lists.deleteInvoiceAddingToStore.Contains(invoice.InvoiceTypeId))
                        {
                            await serialsService.DeleteSerialsForExtractInvoice(invoice.ParentInvoiceCode, invoice.InvoiceType, invoice.StoreId,invoice.InvoiceTypeId);
                        }
                        else if (Lists.deleteInvoiceExtractFromStore.Contains(invoice.InvoiceTypeId))
                        {
                            await serialsService.DeleteSerialsForAddedInvoice(invoice.ParentInvoiceCode, invoice.InvoiceType);

                        }
                    }


                    saved = await InvoiceDetailsRepositoryCommand.AddAsync(invoiceDetailsList);
                    if (!saved)
                        return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };



                    //Add serialize 
                    saved = GeneralAPIsService.addSerialize(invoice.Serialize, item.InvoiceId, invoiceTypeId, invoice.BranchId);
                    if (!saved)
                        return new ResponseResult { Result = Result.Failed, Note = "Faild in add serialize of " + invoice.InvoiceType };

                    if (Lists.purchasesInvoicesList.Contains(invoiceTypeId) || Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId))
                    {
                        //Add Purchase Payment List 
                        var PaymentsMethods = PaymentsMethodsRepositoryQuery.FindAll(q => q.InvoiceId == item.InvoiceId);
                        if (PaymentsMethods.Count() > 0)
                        {
                            var InvoicePaymentMethodList = new List<InvoicePaymentsMethods>();

                            foreach (var itemPayment in PaymentsMethods)
                            {
                                var InvoicePaymentMethod = new InvoicePaymentsMethods();
                                InvoicePaymentMethod.InvoiceId = invoice.InvoiceId;
                                InvoicePaymentMethod.BranchId = invoice.BranchId;
                                InvoicePaymentMethod.Cheque = itemPayment.Cheque;
                                InvoicePaymentMethod.PaymentMethodId = itemPayment.PaymentMethodId;
                                InvoicePaymentMethod.Value = itemPayment.Value;
                                InvoicePaymentMethodList.Add(InvoicePaymentMethod);
                            }
                            saved = await PaymentsMethodsRepositoryCommand.AddAsync(InvoicePaymentMethodList);
                            if (!saved)
                                return new ResponseResult { Result = Result.Failed, Note = "Faild in payment methods of " + invoice.InvoiceType };

                        }
                        //Add Purchase additional costs List 
                        var AdditionalCostsRelations = AdditionalCostsRelationRepositoryQuery.FindAll(q => q.InvoiceId == item.InvoiceId);

                        if (AdditionalCostsRelations.Count() > 0)
                        {
                            var InvoiceAdditionalCostList = new List<InvPurchaseAdditionalCostsRelation>();

                            foreach (var itemAdditionalCosts in AdditionalCostsRelations)
                            {
                                var InvoiceAdditionalCost = new InvPurchaseAdditionalCostsRelation();
                                InvoiceAdditionalCost.InvoiceId = invoice.InvoiceId;
                                InvoiceAdditionalCost.AddtionalCostId = itemAdditionalCosts.AddtionalCostId;
                                InvoiceAdditionalCost.Amount = itemAdditionalCosts.Amount;
                                InvoiceAdditionalCostList.Add(InvoiceAdditionalCost);
                            }
                            InvoiceAdditionalCostsRelationRepositoryCommand.AddRange(InvoiceAdditionalCostList);

                        }

                    }


                    // to update TotalPrice after calculations
                    await InvoiceMasterRepositoryCommand.UpdateAsyn(invoice);
                    var mainSerialize = Math.Truncate(invoice.Serialize);
                    InvoiceMasterRepositoryCommand.CommitTransaction();
                    if (Lists.purchasesInvoicesList.Contains(invoice.InvoiceTypeId) || Lists.salesInvoicesList.Contains(invoice.InvoiceTypeId)
                           || Lists.POSInvoicesList.Contains(invoice.InvoiceTypeId) || Lists.storesInvoicesList.Contains(invoice.InvoiceTypeId)
                           || Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId))
                    {

                        //  foreach (var id in Ids)
                        // {
                        var setJournalEntry = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(new PurchasesJournalEntryIntegrationDTO()
                        {
                            invoiceId = Ids,
                            isDelete = true,
                            branchId = invoice.BranchId
                        });
                        // }
                        if (!Lists.storesInvoicesList.Contains(invoice.InvoiceTypeId))
                        {
                            var setReceipt = await ReceiptsInvoice.DeleteInvoicesReceipts(idS);

                        }

                    }

                    GeneralAPIsService.generateEditedItems(invoiceDetailsList, mainSerialize, false, invoice.InvoiceId,invoice.BranchId);
                    //    HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, "A", null, invoice.BookIndex, invoice.Code
                    //, invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoiceType, (int)SubType.Nothing, invoice.IsDeleted, invoice.ParentInvoiceCode, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);


                    HistoryInvoiceService.HistoryInvoiceMaster(item.BranchId, item.Notes, item.BrowserName, "D", null, item.BookIndex, item.Code
             , item.InvoiceDate, item.InvoiceId, item.InvoiceType, item.InvoiceTypeId, (int)SubType.Nothing, item.IsDeleted, item.InvoiceType, item.Serialize, item.StoreId, item.TotalPrice);

                }
            }
            catch (Exception e)
            {


                throw;
            }

            //            var setReceipt = await ReceiptsInvoice.DeleteInvoicesReceipts(Ids.ToList());

            SystemActionEnum systemActionEnum = new SystemActionEnum();
            if (invoiceTypeId == (int)DocumentType.DeleteSales)
                systemActionEnum = SystemActionEnum.deleteSalesInvoice;
            else if (invoiceTypeId == (int)DocumentType.DeletePurchase)
                systemActionEnum = SystemActionEnum.deletePurchaseInvoice;
            else if (invoiceTypeId == (int)DocumentType.DeleteAddPermission)
                systemActionEnum = SystemActionEnum.deleteAddPermisson;
            else if (invoiceTypeId == (int)DocumentType.DeleteItemsFund)
                systemActionEnum = SystemActionEnum.deleteItemsEntryFund;
            else if (invoiceTypeId == (int)DocumentType.DeletePOS)
                systemActionEnum = SystemActionEnum.deletePOSInvoice;
            else if (invoiceTypeId == (int)DocumentType.DeleteExtractPermission)
                systemActionEnum = SystemActionEnum.deleteExtractPermission;
            else if (invoiceTypeId == (int)DocumentType.DeletedOutgoingTransfer)
                systemActionEnum = SystemActionEnum.DeletedOutgoingTransfer;
            else if (invoiceTypeId == (int)DocumentType.OfferPrice)
                systemActionEnum = SystemActionEnum.deleteOfferPrice;
            else if (invoiceTypeId == (int)DocumentType.DeleteWov_purchase)
                systemActionEnum = SystemActionEnum.deleteWOVPurchase;
            if(systemActionEnum != null)
                await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
            return new ResponseResult
            {
                Result = Result.Success,
                ErrorMessageEn = (invoiceCantDeleted.Count() > 0 ? (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedEn : ErrorMessagesEn.CanNotDeleteSomeInvoices) : ErrorMessagesEn.DeletedSuccessfully),
                ErrorMessageAr = (invoiceCantDeleted.Count() > 0 ? (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedAr : ErrorMessagesAr.CanNotDeleteSomeInvoices) : ErrorMessagesAr.DeletedSuccessfully)

            };

        }


    }
}
