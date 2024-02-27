using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.Serials
{
    public static class CheckSerialMethod
    {
        public static serialsReponse CheckSerial(serialsRequest request, bool fromSavingInvoice, IRepositoryQuery<InvSerialTransaction> serialTransactionQuery, IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
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

            var serialRemovedInEdit_ = false;
            var removedSerials= new List<string>();
            if (request.serialRemovedInEdit != null)
            {
                serialRemovedInEdit_ = request.serialRemovedInEdit.Value;
               if(serialRemovedInEdit_)
                {
                    var serials = serialTransactionQuery.TableNoTracking.Where(a => a.TransferStatus != TransferStatus.Binded && a.AddedInvoice == request.invoiceType)
                    .Select(a => a.SerialNumber).ToList();
                    var repeatedSerials_ = serials.Intersect(newSerials).ToList();
                    removedSerials.AddRange(repeatedSerials_);

                    if(removedSerials.Count() > 0)
                        serialRemovedInEdit_=true;
                }
            }

           
             if (repeatedSerials.Count() > 0 &&  !serialRemovedInEdit_ )
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
    }
}
