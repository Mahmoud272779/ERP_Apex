using App.Application.Handlers.Transfer.OutgoingTransfer.Command;
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Request.Store.Reports.Store;
using FastReport.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Transfer
{
    public class GetAllTransferOutgoing : IRequestHandler<GetAllOutgoingTransferRequest, ResponseResult>
    {
        readonly IAddInvoice generalProcess;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceMaster> invoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly iUserInformation userinformation;

        public GetAllTransferOutgoing(
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery,
                              IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand,
                              IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery,
                              IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery, iUserInformation Userinformation
                           )
        {

            this.generalProcess = generalProcess;
            invoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            invoiceMasterRepositoryCommand = InvoiceMasterRepositoryCommand;
            invGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            userinformation = Userinformation;
            this.invoiceDetailsRepositoryQuery = invoiceDetailsRepositoryQuery;
            this.invoiceDetailsRepositoryCommand = invoiceDetailsRepositoryCommand;
        }

        public async Task<ResponseResult> Handle(GetAllOutgoingTransferRequest request, CancellationToken cancellationToken)
        {
            UserInformationModel userInfo = await userinformation.GetUserInformation();
            //var result = invoiceMasterRepositoryQuery.TableNoTracking
            //    .Where(q => q.BranchId == userInfo.CurrentbranchId &&q.StoreIdTo != null &&
            //              ((q.InvoiceTypeId == (int)DocumentType.OutgoingTransfer && q.IsDeleted == false) ||
            //                q.InvoiceTypeId == (int)DocumentType.DeletedOutgoingTransfer) &&
            //               (!string.IsNullOrEmpty(request.Code) ? (q.Code.ToString() == request.Code || q.BookIndex == request.Code) : 1==1) &&
            //               (request.StoreId.Count==0||request.StoreId.Contains(q.StoreId)) &&
            //               (request.StoreIdTo.Count==0||request.StoreIdTo.Contains(q.StoreIdTo.Value))&&
            //               (request.transferStatus.Count == 0 || request.transferStatus.Contains(q.transferStatus)) &&
            //                (request.DateFrom == null || q.InvoiceDate >= request.DateFrom)
            //     && (request.DateTo == null || q.InvoiceDate <= request.DateTo)

            //                )
            //    .Select(s => new OutGoingTransferResponse
            //    {
            //    InvoiceId=s.InvoiceId,
            //    RecCode=s.InvoiceType,
            //    RecNumber= s.BookIndex,
            //    Date= s.InvoiceDate,
            //    TransStatus= s.transferStatus,
            //    StoreToAR=s.storeTo.ArabicName,
            //    StoreToEN=s.storeTo.LatinName,
            //    StoreFromAR=s.store.ArabicName,
            //    StoreFromEN=s.store.LatinName


            //});

            var result = invoiceMasterRepositoryQuery.TableNoTracking
               .Where(q => q.BranchId == userInfo.CurrentbranchId &&
                         ((q.StoreIdTo != null && q.InvoiceTypeId == request.docTypeId ) || q.InvoiceTypeId == request.DeletedDocTypeId) &&  
                           (request.docTypeId==(int)DocumentType.IncomingTransfer? q.InvoiceSubTypesId==(int)SubType.AcceptedTransfer:true));
         
            if (!string.IsNullOrEmpty(request.Code)) 
                               result=result.Where(q=>  q.Code.ToString() == request.Code || q.BookIndex == request.Code);

           List< int> storeTO = request.StoreIdTo;
           List< int >storeFrom = request.StoreId;

            if (request.docTypeId == (int)DocumentType.IncomingTransfer)
            {
                storeTO = request.StoreId;
                storeFrom = request.StoreIdTo;
            }
            if (storeFrom.Count > 0)
                result = result.Where(q => storeFrom.Contains(q.StoreId));

            if (storeTO.Count > 0)
                result = result.Where(q => storeTO.Contains(q.StoreIdTo.Value));

            if (request.transferStatus.Count > 0)
                result = result.Where(q => request.transferStatus.Contains(q.transferStatus));
            if (request.DateFrom != null)
                result = result.Where(q => q.InvoiceDate >= request.DateFrom);
            if (request.DateTo != null)
                result = result.Where(q => q.InvoiceDate <= request.DateTo);

           var result_=   result.Select(s => new OutGoingTransferResponse
               {
                   InvoiceId = s.InvoiceId,
                   RecCode = s.InvoiceType,
                   RecNumber = s.BookIndex,
                   Date = s.InvoiceDate,
                   TransStatus = s.transferStatus,
                   StoreToAR =(request.docTypeId==(int)DocumentType.IncomingTransfer? s.store.ArabicName : s.storeTo.ArabicName), // بنعكس المخازن فى حاله التحويل المقبول
                   StoreToEN = (request.docTypeId == (int)DocumentType.IncomingTransfer ? s.store.LatinName : s.storeTo.LatinName),
                   StoreFromAR = (request.docTypeId == (int)DocumentType.IncomingTransfer ? s.storeTo.ArabicName : s.store.ArabicName),
                   StoreFromEN = (request.docTypeId == (int)DocumentType.IncomingTransfer ? s.storeTo.LatinName : s.store.LatinName),
                   Code= s.Code
               }).OrderByDescending(a => a.Code);
            var FinalData = Pagenation<OutGoingTransferResponse>.pagenationList(request.pageSize, request.pageNumber, result_);
            int Totalcount = invoiceMasterRepositoryQuery.TableNoTracking
                .Where(q => q.BranchId == userInfo.CurrentbranchId &&
                          ((q.InvoiceTypeId == request.docTypeId && q.IsDeleted == false) ||
                            q.InvoiceTypeId == request.DeletedDocTypeId)).Count();

            return new ResponseResult() { Data = FinalData, DataCount = result.Count(), TotalCount = Totalcount ,Result=Result.Success};
        }
    }
}
