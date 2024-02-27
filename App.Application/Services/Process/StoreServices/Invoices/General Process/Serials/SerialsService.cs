using App.Application.Handlers.InvoicesHelper.Serials;
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials
{
    public class SerialsService : ISerialsService
    {


        private readonly IRepositoryQuery<InvSerialTransaction> serialsTransactionQuery;
        private readonly IRepositoryCommand<InvSerialTransaction> serialsTransactionCommand;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;

        public SerialsService(IRepositoryQuery<InvSerialTransaction> serialsTransactionQuery,
            IRepositoryCommand<InvSerialTransaction> serialsTransactionCommand,
            IGeneralAPIsService generalAPIsService, IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.serialsTransactionQuery = serialsTransactionQuery;
            this.serialsTransactionCommand = serialsTransactionCommand;
            GeneralAPIsService = generalAPIsService;
            this.itemMasterQuery = itemMasterQuery;
            this.itemUnitsQuery = itemUnitsQuery;
        }


        public async Task<string> AddSerialsForAddedInvoice(string addedInvoice, List<InvoiceDetailsRequest> itemsWithSerialsFromRequest, string extractedInvoice, int storeId,int invoiceTypeId , int transStatusOfSerial)
        {
            var itemsEmpty = "";
            try
            {
                var serialTransaction = serialsTransactionQuery.TableNoTracking.ToList();
               var serialsFromDb = serialTransaction.Where(a => a.AddedInvoice == addedInvoice && a.IsDeleted == false).ToList();//&& string.IsNullOrEmpty(a.ExtractInvoice)).ToList();

                var serialsFromRequest = new List<InvSerialTransaction>();
                foreach (var item in itemsWithSerialsFromRequest)
                {
                    if (item.ListSerials.Count() == 0 && invoiceTypeId!=(int)DocumentType.IncomingTransfer)
                        itemsEmpty = "serials of item " + item.ItemCode + " is empty";

                    //add all serials of request to list of InvSerialTransaction
                    foreach (var serial in item.ListSerials)
                    {
                        var serial_ = new InvSerialTransaction();
                        serial_.ItemId = item.ItemId;
                        serial_.AddedInvoice = addedInvoice;
                        serial_.SerialNumber = serial.ToUpper();
                        serial_.StoreId = storeId;
                        serial_.ExtractInvoice = extractedInvoice;
                        serial_.indexOfSerialForAdd = item.IndexOfItem;
                        serial_.TransferStatus = transStatusOfSerial;
                        serialsFromRequest.Add(serial_);
                    }
                    serialsFromDb.Where(a => item.ListSerials.Contains(a.SerialNumber))
                        .Select(a => { a.indexOfSerialForAdd = item.IndexOfItem;a.StoreId = storeId; a.ItemId = item.ItemId;  return a; }).ToList();
                }
                // لتعديل السيريالات اللى محصلش عليها تغيير 
                if (invoiceTypeId != (int)DocumentType.IncomingTransfer)
                   await  serialsTransactionCommand.UpdateAsynWithOutSave(serialsFromDb);
                 else
                    serialsTransactionCommand.UpdateAsynWithOutSave(serialsFromDb);

                serialsFromDb.Where(a => string.IsNullOrEmpty(a.ExtractInvoice)).ToList();
                // in case of adding new serials 
                var serialsWillAdded = serialsFromRequest.Where(a => !serialsFromDb.Select(e => e.SerialNumber).ToList().Contains(a.SerialNumber)).ToList();
                serialsTransactionCommand.AddRangeAsync(serialsWillAdded);

                // in case of deleting serials from the invoice
                // منعت تعديل فى حاله التحويل الوارد المرفوض لان المقبول و المرفوض بياخدو نفس كود الفاتوره ف بيعتبر المقبول انه اتحذف من الفاتورة 
                if(invoiceTypeId!= (int)DocumentType.IncomingTransfer)
                {
                    var serialsWillUpdated = serialsFromDb.Where(a => !serialsFromRequest.Select(e => e.SerialNumber).ToList().Contains(a.SerialNumber)).ToList();
                    serialsWillUpdated.Select(a => { a.IsDeleted = true; return a; }).ToList();
                    if(invoiceTypeId== (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.AddPermission || invoiceTypeId == (int)DocumentType.itemsFund)
                    {
                        // لو حذفت سيريال فى المشتريات كان داخل المخزن من خلال حذف او مرتجع مبيعات
                        // عشان امنع ظهوره مره تانيه فى المبيعات 
                        var serialsWillUpdates_ = serialsWillUpdated.Select(a => a.SerialNumber);
                         var serialRemoveFromStore = serialTransaction.Where(a => a.ExtractInvoice == null
                                                && serialsWillUpdates_.Contains(a.SerialNumber)).ToList();
                        serialRemoveFromStore.Select(a => { a.ExtractInvoice = Actions.deleteSerial; return a; }).ToList();
                        if (serialRemoveFromStore.Any())
                            serialsWillUpdated.AddRange(serialRemoveFromStore);
                       //     await serialsTransactionCommand.UpdateAsynWithOutSave(serialRemoveFromStore);

                    }

                    if (serialsWillUpdated.Count()>0)
                        await   serialsTransactionCommand.UpdateAsynWithOutSave(serialsWillUpdated);
                //    await serialsTransactionCommand.SaveAsync();

                }

                  serialsTransactionCommand.SaveChanges();

                //    await  serialsTransactionCommand.UpdateAsyn(serialsFromRequest);
                return itemsEmpty;
            }
            catch (Exception e)
            {
                itemsEmpty = e.Message;
            }
            // var serialsWillDeleted = serialsFromDb.Where(a => !itemsWithSerialsFromRequest.Select(e => e.ItemId).Contains(a.ItemId));

            return itemsEmpty;
        }

        public async Task<Tuple<bool, string, string>> AddSerialsForExtractInvoice(bool isUpdateOrDelete, List<InvoiceDetailsRequest> itemsWithSerialsFromRequest, string extractedInvoice, int? transferStatus)
        {
            var serialsFromRequest = new List<InvSerialTransaction>();
            foreach (var item in itemsWithSerialsFromRequest)
            {
                foreach (var serial in item.ListSerials)
                {
                    var serial_ = new InvSerialTransaction();
                    serial_.ItemId = item.ItemId;
                    serial_.SerialNumber = serial.ToUpper();
                    serial_.indexOfSerialForExtract = item.IndexOfItem;
                    serial_.TransferStatus = item.TransStatus;
                    serialsFromRequest.Add(serial_);
                }
            }

            var serialsFromDb = serialsTransactionQuery.TableNoTracking.ToList();//(a => (isUpdateOrDelete ? a.ExtractInvoice == extractedInvoice : true));
                                                                                 //                    .Where(a=> (isUpdateOrDelete ? a.ExtractInvoice==extractedInvoice: true));



            // in case of add or update
            var serialsWillAdded = serialsFromDb.Where(a => serialsFromRequest.Select(e => e.ItemId).Contains(a.ItemId));
            //   && (isUpdate ? a.ExtractInvoice == extractedInvoice : true));

            var SerialsWillAccridited = serialsWillAdded.Where(a => serialsFromRequest.Select(e => e.SerialNumber).Contains(a.SerialNumber) && a.ExtractInvoice == null).ToList();
            SerialsWillAccridited.Select(a =>
            {
                a.IsAccridited = true; a.ExtractInvoice = extractedInvoice;
                a.TransferStatus = transferStatus.Value; return a;
            }).ToList();

            SerialsWillAccridited.ForEach(a => a.indexOfSerialForExtract =
                 serialsFromRequest.Where(e => e.SerialNumber == a.SerialNumber).Select(e => e.indexOfSerialForExtract).First());
            var saved = await serialsTransactionCommand.UpdateAsynWithOutSave(SerialsWillAccridited);
            // in case of deleting or update serials from the invoice
            if (isUpdateOrDelete)
            {
                // check removed serials if Added Later or not
                var oldSerials = serialsFromDb.Where(a => a.ExtractInvoice == extractedInvoice).ToList();


                var removedSerials = oldSerials.Select(a => a.SerialNumber).ToList()
                    .Except(serialsFromRequest.Select(a => a.SerialNumber).ToList());
                var removedSerialsList = oldSerials.Where(a => removedSerials.Contains(a.SerialNumber));
                foreach (var s in removedSerialsList)
                {
                    var serialRepeated = serialsFromDb.Where(a => a.SerialNumber == s.SerialNumber && a.ExtractInvoice == null);
                    if (serialRepeated.Count() > 0)
                        return new Tuple<bool, string, string>(false, ErrorMessagesAr.CanNotRemoveSerial, ErrorMessagesEn.CanNotRemoveSerial);

                }

                // to update index of serials on the invoice
                var _oldSerial = oldSerials.Except(removedSerialsList).ToList();
                _oldSerial.ForEach(a => a.indexOfSerialForExtract =
                (serialsFromRequest.Count() > 0 ? serialsFromRequest.Where(e => e.SerialNumber == a.SerialNumber).Select(e => e.indexOfSerialForExtract).First() : 0));
                saved = await serialsTransactionCommand.UpdateAsynWithOutSave(oldSerials);

                //old
                // removedSerialsList.Select(a => a.ExtractInvoice == null).ToList();

                // new
                removedSerialsList = removedSerialsList.Where(a => a.IsDeleted == false).Select(a => { a.ExtractInvoice = null; return a; }).ToList();

                removedSerialsList.Where(a => a.IsDeleted == false).ToList()
                      .ForEach(h => { h.indexOfSerialForExtract = 0; h.IsAccridited = false; h.TransferStatus = 0; });
                saved = await serialsTransactionCommand.UpdateAsynWithOutSave(removedSerialsList);

                var serialsWillUpdated = serialsFromDb.Where(a => (isUpdateOrDelete ? a.ExtractInvoice == extractedInvoice : true) && !serialsFromRequest.Select(e => e.SerialNumber).ToList().Contains(a.SerialNumber)).ToList();
                serialsWillUpdated.Select(a => { a.IsDeleted = true; return a; }).ToList();
                saved = await serialsTransactionCommand.UpdateAsynWithOutSave(serialsWillUpdated);

            }
            serialsTransactionCommand.SaveChanges();
            return new Tuple<bool, string, string>(saved, "", "");

        }


        public async Task<bool> DeleteSerialsForAddedInvoice(string ParentInvoice, string ExtractedInvoice)
        {
            //var serialsWillDeleted = serialsTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == ParentInvoice &&
            //         a.ExtractInvoice == null).ToList();
            var serialsOfInvoice = serialsTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == ParentInvoice).Select(a=>a.SerialNumber);
            var serialsWillDeleted = serialsTransactionQuery.TableNoTracking.Where(a => serialsOfInvoice.Contains(  a.SerialNumber)  &&
                   a.ExtractInvoice == null).ToList();

            serialsWillDeleted.Select(a => { a.IsAccridited = true; a.IsDeleted = true; a.ExtractInvoice = ExtractedInvoice;
                a.indexOfSerialForExtract = a.indexOfSerialForAdd;  return a; }).ToList();
            var save = await serialsTransactionCommand.UpdateAsyn(serialsWillDeleted);
            return save;
        }

        public async Task<string> DeleteSerialsForExtractInvoice(string ParentInvoice, string AddedInvoice, int storeId,int? invoiceTypeId)
        {
            var serialsInDb = serialsTransactionQuery.TableNoTracking.Where(a => a.ExtractInvoice == ParentInvoice
                && a.IsDeleted == false).ToList();//.GroupBy(a => a.ItemId);
            var serialsInDbGroup= serialsInDb.GroupBy(a=> a.ItemId);
            var itemsWithSerialsFromRequest = new List<InvoiceDetailsRequest>();

            foreach (var item in serialsInDbGroup)
            {
                var itemData = new InvoiceDetailsRequest();
                itemData.ItemId = item.Key;
                itemData.IndexOfItem = item.First().indexOfSerialForExtract;
                itemData.ListSerials = item.Select(a => a.SerialNumber).ToList();
                itemsWithSerialsFromRequest.Add(itemData);
            }
            var saved = await AddSerialsForAddedInvoice(AddedInvoice, itemsWithSerialsFromRequest, null, storeId,invoiceTypeId.Value,0);
            if(invoiceTypeId.Value==(int)DocumentType.DeletedOutgoingTransfer)
            {
                serialsInDb.Select(a => a.TransferStatus = 0).ToList();
                await serialsTransactionCommand.UpdateAsyn(serialsInDb);
            }

            return saved;
        }

        public bool SerialsExist(int invoiceTypeId, string invoiceType, List<InvoiceDetailsRequest> InvoicesDetails)
        {
            if (Lists.InvoicesTypeOfAddingToStore.Contains(invoiceTypeId))
            {
                foreach (var item in InvoicesDetails)
                {
                    if (item.ItemTypeId == (int)ItemTypes.Serial)
                    {
                        serialsRequest serial_request = new serialsRequest();
                        serial_request.newEnteredSerials.Select(a => a.SerialNumber).ToList().AddRange(item.ListSerials);
                        serial_request.isDiffNumbers = true;
                        serial_request.invoiceType = invoiceType;
                        //  serial_request.serialsInTheSameInvoice = parameter.serialsInTheSameInvoice;
                        serialsReponse serialsResult = CheckSerialMethod.CheckSerial(serial_request,true, serialsTransactionQuery, itemMasterQuery, itemUnitsQuery);
                        if (serialsResult.serialsStatus != (int)SerialsStatus.accepted || item.ListSerials.Count() == 0)
                            return true;
                    }

                }

            }
            else
            {

            }

            return false;

        }
        
        public async Task<ResponseResult> checkSerialBeforeSave(bool isUpdate, string? invoiceType, List<InvoiceDetailsRequest>? invoiceDetails, int invoicceTypeId, int storeId , int oldStore)
        {

         
            var serialsListRequest = new List<string>();

            invoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).Select(a => a.ListSerials).ToList()
            .ForEach(a => serialsListRequest.AddRange(a));
           
            if(serialsListRequest.Count()==0 && !isUpdate)
            return new ResponseResult { Result = Result.Success };

            serialsListRequest = serialsListRequest.ConvertAll(d => d.ToUpper());

            var DistinctSerials = serialsListRequest.Distinct().ToList();
            if(DistinctSerials.Count()!= serialsListRequest.Count())
                return new ResponseResult
                {
                    Result = Result.SerialsConflicted,
                    ErrorMessageAr = ErrorMessagesAr.SerialsRepeatedIntheSameInvoice,
                    ErrorMessageEn = ErrorMessagesEn.SerialsRepeatedIntheSameInvoice
                };

            var serials_Binded = new List<serials_binded>();

           
            if (invoicceTypeId!= (int)DocumentType.IncomingTransfer)
            {
              /*  var serialsBinded = await GeneralAPIsService.serialIsBinded("", serialsListRequest, invoiceType, invoiceDetails);
                if (serialsBinded.Count() > 0)
                {
                    string serialsBinded_ = string.Join(",", serialsBinded.ToArray());
                    return new ResponseResult() { Result = Result.bindedTransfer, ErrorMessageAr = ErrorMessagesAr.SerialsBinded + "  " + serialsBinded_, ErrorMessageEn = ErrorMessagesEn.SerialsBinded + "  " + serialsBinded_ };
                }*/
            }
           

            if (Lists.InvoicesTypeOfAddingToStore.Contains(invoicceTypeId))
            {
                if (isUpdate)
                {
                    var serialsOfInvoiceFromDb = serialsTransactionQuery.TableNoTracking.Where(a => a.AddedInvoice == invoiceType && a.IsDeleted == false);
                    var serialNumberDb = serialsOfInvoiceFromDb.Select(a => a.SerialNumber).ToList();
                    //get removed serials from updated invoice
                    var serialsRemoved = (serialsListRequest.Count() > 0 ? serialNumberDb.Except(serialsListRequest) : serialNumberDb);

                    // check if can remove this serials    
                    var serialsexist = serialsTransactionQuery.TableNoTracking.Where(a =>a.StoreId==storeId &&
                    serialsRemoved.Contains(a.SerialNumber)
                       && (a.ExtractInvoice == null)).Select(a => a.SerialNumber).ToList();
                    if (serialsexist.Count() != serialsRemoved.Count())
                    {
                        var serialsCantRemove = serialsRemoved.Except(serialsexist);
                        string serialsRemovedOut = string.Join(",", serialsCantRemove.ToArray());
                        return new ResponseResult { Data = serialsCantRemove, Result = Result.SerialExist, ErrorMessageAr = ErrorMessagesAr.CanNotRemoveSerial + "  " + serialsRemovedOut, ErrorMessageEn = ErrorMessagesEn.CanNotRemoveSerial + "  " + serialsRemovedOut };
                    }

                    var serialsAdded = serialsListRequest.Except(serialNumberDb);


                    var serialsCantAdded = serialsTransactionQuery.TableNoTracking.Where(a => serialsAdded.Contains(a.SerialNumber) && a.ExtractInvoice == null && !a.IsDeleted).Select(a => a.SerialNumber);

                    if (serialsCantAdded.Count() > 0)
                    {
                        string SerialExistOut = string.Join(",", serialsCantAdded.ToArray());

                        return new ResponseResult { Data = serialsCantAdded, Result = Result.SerialExist, ErrorMessageAr = ErrorMessagesAr.SerialExist, ErrorMessageEn = ErrorMessagesEn.SerialExist };

                    }
                    if(storeId != oldStore)
                    {

                        var serialExtracted = serialsOfInvoiceFromDb.Where(a => a.ExtractInvoice != null  && a.StoreId == oldStore).Select(a => a.SerialNumber);//);
                        if(serialExtracted.Count()>0)
                        {
                            return new ResponseResult { Result = Result.canNotChangeStore, ErrorMessageAr = ErrorMessagesAr.CanNotChangeStore, ErrorMessageEn = ErrorMessagesEn.CanNotChangeStore }; 

                        }

                        #region changeStore
                        /* مؤجله حين اشعار اخر
                             بتضرب فى الكيس دي
                            فى المخزن الرئيسي اشتريت 4 سيريالات وبعدين بيعت 1 فيهم  
                             وحذفت البيع ينفع اغير المخزن فى الشراء بس اللى حذفته من مبيعات المخزن الرئيسي لسه موجود فى الرئيسي
                        */
                        /* var serialExistInStore = serialsTransactionQuery.TableNoTracking.Where(a => a.ExtractInvoice == null && a.StoreId == oldStore && serialExtracted.Contains(a.SerialNumber));
                         if (serialExtracted.Count() != serialExistInStore.Count())
                             return new ResponseResult { Result = Result.canNotChangeStore, ErrorMessageAr = ErrorMessagesAr.CanNotChangeStore, ErrorMessageEn = ErrorMessagesEn.CanNotChangeStore };*/
                        #endregion
                    }
                    // serialsExist = serialsExist.Where(a => a.AddedInvoice != invoiceType);

                }
                else
                {
                    // السيريال موجود فى السيريالات
                    var serialsExist = serialsTransactionQuery.TableNoTracking
                        .Where(a => serialsListRequest.Contains(a.SerialNumber) //&& a.StoreId == storeId
                                      && ((a.ExtractInvoice == null && !a.IsDeleted)||
                                     (//a.TransferStatus == TransferStatus.Binded
                                     (invoicceTypeId!=(int)DocumentType.IncomingTransfer? 
                                     a.TransferStatus == TransferStatus.Binded 
                                     :false)
                                      && a.ExtractInvoice != null)) ).ToList();
                    //  .Select(a=>a.SerialNumber).ToList();
                    if (serialsExist.Count() > 0)
                    {
                        var _serialsExist = serialsExist.Select(a => a.SerialNumber).ToList();
                       
                        // var serialsCantAdded = serialsListRequest.Except(_serialsExist);
                        //if (serialsCantAdded.Count() > 0)
                        if (_serialsExist.Count() > 0)
                        {
                            string SerialExistOut = string.Join(",", _serialsExist.ToArray());

                            return new ResponseResult { Data = SerialExistOut, Result = Result.SerialExist, ErrorMessageAr = ErrorMessagesAr.SerialExist + SerialExistOut, ErrorMessageEn = ErrorMessagesEn.SerialExist + SerialExistOut };
                        }

                    }

                }




            }
            else if (Lists.InvoicesTypesOfExtractFromStore.Contains(invoicceTypeId))
            {
                var serialsFromDb = serialsTransactionQuery.TableNoTracking.Where(a => serialsListRequest.Contains(a.SerialNumber)
                && a.StoreId == storeId
                   && a.AddedInvoice != null && (a.ExtractInvoice == null || a.ExtractInvoice == invoiceType) && a.IsDeleted == false).ToList();


                if (serialsFromDb.Count() != serialsListRequest.Count())
                {
                    var serials = serialsListRequest.Where(a => !serialsFromDb.Select(a => a.SerialNumber).ToList().Contains(a)).ToList();
                    string serialsNotExist = string.Join(",", serials.ToArray());
                    return new ResponseResult { Data = serialsFromDb, Result = Result.SerialNotExist, ErrorMessageAr = ErrorMessagesAr.SerialNotExist + "  " + serialsNotExist, ErrorMessageEn = ErrorMessagesEn.SerialNotExist + "  " + serialsNotExist };

                }

                //  منع ادخال سيرال من صنف ف صنف اخر
                var serialsInItemsDb = serialsFromDb.Select(a => new { a.ItemId, a.SerialNumber }).ToList();
                var serialsInItemsRequest = invoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial)
                         .Select(a => new { a.ItemId, a.ListSerials }).ToList();
                var serialsConflicted = new List<string>();
                foreach (var item in serialsInItemsRequest)
                {
                    var ListSerials = item.ListSerials.ConvertAll(d => d.ToUpper());
                    var serialsRight = serialsInItemsDb.Where(a => a.ItemId == item.ItemId && ListSerials.Contains(a.SerialNumber))
                        .Select(a => a.SerialNumber);
                   
                    var serialsNotExistINSameItem = ListSerials.Except(serialsRight);
                    if (serialsNotExistINSameItem.Count() > 0)
                        serialsConflicted.AddRange(serialsNotExistINSameItem);

                }

                if (serialsConflicted.Count() > 0)
                {
                    string serialsConflicted_ = string.Join(",", serialsConflicted.ToArray());
                    return new ResponseResult { Data = serialsConflicted_, Result = Result.SerialsConflicted, ErrorMessageAr = ErrorMessagesAr.SerialsConflicted + "  " + serialsConflicted_, ErrorMessageEn = ErrorMessagesEn.SerialsConflicted + "  " + serialsConflicted_ };

                }

            }

            // check if serials exist in itemCode , nationalBracode or barcode
            var serialsInItemsCode = itemMasterQuery.TableNoTracking.Where(a => serialsListRequest.Contains(a.ItemCode)
                                                || serialsListRequest.Contains(a.NationalBarcode));
            var serialsInBarcode = itemUnitsQuery.TableNoTracking.Where(a => serialsListRequest.Contains(a.Barcode)).Select(a => a.Barcode);

            if (serialsInItemsCode.Count() > 0 || serialsInBarcode.Count() > 0)
            {

                var itemCode = serialsInItemsCode.Where(a => serialsListRequest.Contains(a.ItemCode)).Select(a => a.ItemCode);
                var serialsExist = itemCode.ToList();
                var itemNationalCode = serialsInItemsCode.Where(a => serialsListRequest.Contains(a.NationalBarcode)).Select(a => a.NationalBarcode);
                serialsExist.AddRange(itemNationalCode.ToList());
                serialsExist.AddRange(serialsInBarcode.ToList());
                if (serialsExist.Count() > 0)
                {
                    string SerialExistOut = string.Join(",", serialsExist.ToArray());

                    return new ResponseResult { Data = SerialExistOut, Result = Result.SerialExist, ErrorMessageAr = ErrorMessagesAr.serialsExistInBarcode, ErrorMessageEn = ErrorMessagesEn.serialsExistInBarcode };
                }
            }

            return new ResponseResult { Result = Result.Success };


        }
   
        public class serials_binded
        {
            public string serial { get; set; }
            public int indexItem { get; set; }
        }
    }
}
