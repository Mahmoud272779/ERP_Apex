using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Infrastructure;
using MediatR;
using SpreadsheetLight;
using System.Linq;
using System.Threading;

namespace App.Application.Handlers.Transfer
{
    public class AddIncommingTransferService : IRequestHandler<AddIncommingTransferRequest, ResponseResult>
    {

        private readonly IAddInvoice generalProcess;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceMaster> invoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;
        private readonly IRepositoryCommand<InvSerialTransaction> serialTransactionCommand;
        private readonly IRedefineInvoiceRequestService redefineInvoiceRequestService;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;


        public AddIncommingTransferService(
                              IAddInvoice generalProcess, IGeneralAPIsService _generalAPIsService,
                              IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery,
                              IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand,
                              IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery,
                              IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IRepositoryQuery<InvSerialTransaction> serialTransactionQuery,
                               IRepositoryCommand<InvSerialTransaction> serialTransactionCommand,
                                IRedefineInvoiceRequestService redefineInvoiceRequestService, IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository
                           )
        {

            this.generalProcess = generalProcess;
            generalAPIsService = _generalAPIsService;
            invoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            invoiceMasterRepositoryCommand = InvoiceMasterRepositoryCommand;
            invGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.invoiceDetailsRepositoryQuery = invoiceDetailsRepositoryQuery;
            this.invoiceDetailsRepositoryCommand = invoiceDetailsRepositoryCommand;
            this.serialTransactionQuery = serialTransactionQuery;
            this.serialTransactionCommand = serialTransactionCommand;
            this.redefineInvoiceRequestService = redefineInvoiceRequestService;
            this.itemCardMasterRepository = itemCardMasterRepository;
        }
        public async Task<ResponseResult> Handle(AddIncommingTransferRequest invoiceRequest, CancellationToken cancellationToken)
        {
            var setting = await invGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

            var invoiceOutgoing = await invoiceMasterRepositoryQuery.GetByAsync(a => a.InvoiceId == invoiceRequest.OutgoingInoviceID);

            //check if qty of items in store or not 
            var outgoingDetails = await invoiceDetailsRepositoryQuery.FindByAsyn(a => a.InvoiceId == invoiceRequest.OutgoingInoviceID);

            InvoiceMasterRequest request = new InvoiceMasterRequest();
            Mapping.Mapper.Map<AddIncommingTransferRequest, InvoiceMasterRequest>(invoiceRequest, request);
            var redefineInvoiceRequest = await redefineInvoiceRequestService.setInvoiceRequest(request, setting,(int)DocumentType.IncomingTransfer,null, invoiceOutgoing.InvoiceType,null,0);
            if (redefineInvoiceRequest.Item2 != "")
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = redefineInvoiceRequest.Item2, ErrorMessageEn = redefineInvoiceRequest.Item3 };
            request = redefineInvoiceRequest.Item1;


            int status = GetSTatus(request.InvoiceDetails.Sum(a => a.Quantity*a.ConversionFactor), outgoingDetails.Sum(a => a.Quantity*a.ConversionFactor));
            request.transferStatus = status;
            //invoiceOutgoing.transferStatus = status;
            request.ParentInvoiceCode = invoiceOutgoing.InvoiceType;

            //validation
            var validation = ValidateTransfer(request, invoiceOutgoing, outgoingDetails);
            if (validation.Result.Result != Result.Success)
                return validation.Result;

            // اضافه الكميه الصادره فى الكميه المحوله لكل صنف
             request.InvoiceDetails.Select(a => a.TransQuantity = outgoingDetails.Where(e => e.ItemId == a.ItemId && //  e.UnitId == a.UnitId && 
                   e.indexOfItem == a.IndexOfItem).Select(e => e.Quantity * e.ConversionFactor/a.ConversionFactor).Sum()).ToList();

            // نحديد حالة الصنف
            request.InvoiceDetails.Select(a => a.TransStatus = GetSTatus(a.Quantity, a.TransQuantity)).ToList();
            request.InvoiceDetails.Select(a => a.Price = outgoingDetails.Where(e => e.indexOfItem == a.IndexOfItem)
            .Select(e => e.Price).First()).ToList();
            InvoiceMasterRequest AcceptedTransferRequest = new InvoiceMasterRequest();
            #region Accepted
            Mapping.Mapper.Map<InvoiceMasterRequest, InvoiceMasterRequest>(request, AcceptedTransferRequest);
            AcceptedTransferRequest.StoreId = invoiceOutgoing.StoreIdTo.Value;
            AcceptedTransferRequest.StoreIdTo = invoiceOutgoing.StoreId;
            AcceptedTransferRequest.transferStatus = status;
            AcceptedTransferRequest.InvoiceDetails = request.InvoiceDetails;

          // update mainoutcoming 
            outgoingDetails.Select(a => a.TransQuantity = (AcceptedTransferRequest.InvoiceDetails.
                                                        Where(e => e.ItemId == a.ItemId
                                                       && e.IndexOfItem == a.indexOfItem).Select(e => e.Quantity * e.ConversionFactor/a.ConversionFactor).Sum())).ToList();

            outgoingDetails.Select(a => a.StatusOfTrans = (AcceptedTransferRequest.InvoiceDetails.
                              Where(e => e.ItemId == a.ItemId
                         && e.IndexOfItem == a.indexOfItem).Select(e => e.TransStatus).FirstOrDefault())).ToList();
            //save
            var resAccepted = await generalProcess.SaveInvoice(AcceptedTransferRequest, setting, null, (int)DocumentType.IncomingTransfer, InvoicesCode.IncomingTransfer, invoiceRequest.OutgoingInoviceID, FilesDirectories.Sales,(int)TransferStatus.Accepted);
            if (resAccepted.Result != Result.Success)
                return resAccepted;

            #endregion
  

            //فى حاله ال partial and rejected
            InvoiceMasterRequest RejectedTransferRequest = new InvoiceMasterRequest();
            #region partialAndRejected 
            Mapping.Mapper.Map<InvoiceMasterRequest, InvoiceMasterRequest>(request, RejectedTransferRequest);
            RejectedTransferRequest.StoreId = invoiceOutgoing.StoreId;
            RejectedTransferRequest.StoreIdTo = invoiceOutgoing.StoreIdTo;
            RejectedTransferRequest.transferStatus = status;

            List<InvoiceDetailsRequest> RejectedTransDetailsRequest = new List<InvoiceDetailsRequest>();
            RejectedTransDetailsRequest.AddRange(request.InvoiceDetails.ToList());

            // حساب الكمية المرفوضه من كل صنف وارتجاعها للمخزن المحول منه
            RejectedTransDetailsRequest.Select(a => a.Quantity = (outgoingDetails
                                      .Where(e => e.ItemId == a.ItemId //&& e.UnitId == a.UnitId 
                                      && e.indexOfItem == a.IndexOfItem)
                                      .Select(e => e.Quantity * e.ConversionFactor / a.ConversionFactor).Sum()) - a.Quantity).ToList();
            //foreach(var item in RejectedTransDetailsRequest)
            //{
            //    item.Quantity = (outgoingDetails
            //                          .Where(e => e.ItemId == item.ItemId //&& e.UnitId == a.UnitId 
            //                          && e.indexOfItem == item.IndexOfItem)
            //                          .Select(e => e.Quantity * e.ConversionFactor / item.ConversionFactor).Sum()) - item.Quantity;
            //}
            // السيريالات المقبوله 
            var acceptedSerials = new List<string>();
            AcceptedTransferRequest.InvoiceDetails.Where(a => a.ItemTypeId == (int)ItemTypes.Serial).Select(a => a.ListSerials).ToList()
           .ForEach(a => acceptedSerials.AddRange(a));

            // سيريالات التحويل الصادر 
         //   var serialsFromOutGoing = serialTransactionQuery.GetByAsync(a => a.ExtractInvoice == invoiceOutgoing.InvoiceType);
           var serialsFromOutGoing = serialTransactionQuery.TableNoTracking.Where(a => a.ExtractInvoice == invoiceOutgoing.InvoiceType).ToList();
            // السيريالات المرفوضه 
            var rejectedSerials = serialsFromOutGoing.Where(a=> ! acceptedSerials.Contains(a.SerialNumber)) ;
            // set rejected serials foreach item in rejected items
            RejectedTransDetailsRequest.Select(a => a.ListSerials = new List<string>()).ToList();
            RejectedTransDetailsRequest.ForEach(e=> e.ListSerials.AddRange(rejectedSerials.Where(a=>a.ItemId== e.ItemId && a.indexOfSerialForExtract==e.IndexOfItem)
                                        .Select(a=>a.SerialNumber)));
            // في حالة الكميه المرفوضه ب0 همسحها من الليست
            RejectedTransDetailsRequest.RemoveAll(a => a.Quantity == 0);
            if (RejectedTransDetailsRequest.Count() > 0)
            {
                RejectedTransferRequest.InvoiceDetails = RejectedTransDetailsRequest;
                //save
                var resRejected = await generalProcess.SaveInvoice(RejectedTransferRequest, setting, null, (int)DocumentType.IncomingTransfer, InvoicesCode.IncomingTransfer, invoiceRequest.OutgoingInoviceID, FilesDirectories.Sales, true, invoiceOutgoing.StoreIdTo, resAccepted.Code,(int)TransferStatus.Rejected);
                if (resRejected.Result != Result.Success)
                    return resRejected;
            }
        
            serialsFromOutGoing.Select(a => a.TransferStatus = 0).ToList();
            await serialTransactionCommand.UpdateAsyn(serialsFromOutGoing);

            #endregion

            #region




            request.InvoiceDetails = request.InvoiceDetails.Where(a => !Lists.itemThatNotTransfer.Contains(a.ItemTypeId)).ToList();


            #endregion
            invoiceOutgoing.transferStatus = status;
            try
            {
                var saved = await  invoiceMasterRepositoryCommand.UpdateAsyn(invoiceOutgoing);

            }
            catch (Exception e )
            {

                throw;
            }
           // invoiceMasterRepositoryCommand.SaveAsync();
            // outgoingDetails.Select(a => a.TransQuantity = (AcceptedTransferRequest.InvoiceDetails.
            //                                             Where(e => e.ItemId == a.ItemId && e.UnitId == a.UnitId
            //                                            && e.IndexOfItem == a.indexOfItem).Select(e => e.Quantity).Sum())).ToList();

            // outgoingDetails.Select(a => a.StatusOfTrans = (AcceptedTransferRequest.InvoiceDetails.
            //                   Where(e => e.ItemId == a.ItemId && e.UnitId == a.UnitId
            //              && e.IndexOfItem == a.indexOfItem).Select(e => e.TransStatus).First())).ToList();

            await invoiceDetailsRepositoryCommand.UpdateAsyn(outgoingDetails);
            return resAccepted;
        }

        private async Task<ResponseResult> ValidateTransfer(InvoiceMasterRequest request, InvoiceMaster invoiceOutgoing, ICollection<InvoiceDetails> outgoingDetails)
        {
            if (invoiceOutgoing == null)
                return new ResponseResult() { };


            if (invoiceOutgoing.IsDeleted == true)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = " لا يمكن استلام تحويل تم حذفه  ", ErrorMessageEn = " can not accept transfer that is deleted " };

            if (invoiceOutgoing.transferStatus != Aliases.TransferStatus.Binded)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = " لا يمكن استلام تحويل تم قبوله من قيل", ErrorMessageEn = " can not accept transfer that is already accepted" };

            //check if qty of items in store or not 
            bool theSameItem = generalAPIsService.checkitemsFromOutgoingTransfer(request.InvoiceDetails, outgoingDetails);
            if (!theSameItem)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = " ليست نفس الاصناف المحوله    ", ErrorMessageEn = " not the same items " };

           
            var qtyNotExist = outgoingDetails.Where(a => a.Quantity <
                           request.InvoiceDetails.Where(e => e.ItemId == a.ItemId && e.UnitId == a.UnitId && e.IndexOfItem == a.indexOfItem)
                           .Select(e => e.Quantity*e.ConversionFactor/a.ConversionFactor).Sum()).Select(a=>a.ItemId);

            if (qtyNotExist.Count() > 0)
            {
                var ItemsNotAvailableName = itemCardMasterRepository.TableNoTracking.Where(a => qtyNotExist.Contains(a.Id));
                var ItemsNotAvailableNameAr = string.Join(",", ItemsNotAvailableName.Select(a=>a.ArabicName));
                var ItemsNotAvailableNameEn = string.Join(",", ItemsNotAvailableName.Select(a => a.LatinName));
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr ="  الكميه اكبر من المحوله للصنف "+ ItemsNotAvailableNameAr,
                    ErrorMessageEn = " Quantity is greater than Transfer for item  " + ItemsNotAvailableNameEn  };

            }

            return new ResponseResult() { Result = Result.Success };
        }


        private int GetSTatus(double IncomingQTY, double OutGoingQTY)
        {
            if (IncomingQTY == OutGoingQTY)
                return Aliases.TransferStatus.Accepted;

            if (IncomingQTY == 0)
                return Aliases.TransferStatus.Rejected;

            return Aliases.TransferStatus.PartialAccepted;


        }


        //throw new NotImplementedException();
    }
}

