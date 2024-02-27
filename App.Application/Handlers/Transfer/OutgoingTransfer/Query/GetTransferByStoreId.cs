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
    public class GetTransferByStoreId : IRequestHandler<GetAllOutgoingByStoreID, ResponseResult>
    {
        readonly IAddInvoice generalProcess;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceMaster> invoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly iUserInformation userinformation;

        public GetTransferByStoreId(
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

        public async Task<ResponseResult> Handle(GetAllOutgoingByStoreID request, CancellationToken cancellationToken)
        {
            UserInformationModel userInfo = await userinformation.GetUserInformation();
            var result = invoiceMasterRepositoryQuery.TableNoTracking
                .Where(q => q.StoreIdTo != null && q.transferStatus == Aliases.TransferStatus.Binded &&
                          ((q.InvoiceTypeId == (int)DocumentType.OutgoingTransfer && q.IsDeleted == false)  &&
                           (q.StoreIdTo.Value==request.StoreId
               
                            )))
                .Select(s => new OutGoingTransferResponse
                {
                InvoiceId=s.InvoiceId,
                RecCode=s.InvoiceType,
                RecNumber= s.BookIndex,
                Date= s.InvoiceDate,
                TransStatus= s.transferStatus,
                StoreToAR=s.storeTo.ArabicName,
                StoreToEN=s.storeTo.LatinName,
                StoreFromAR=s.store.ArabicName,
                StoreFromEN=s.store.LatinName
                ,branchId=s.BranchId

            }).OrderByDescending(a=>a.InvoiceId);
            var FinalData = Pagenation<OutGoingTransferResponse>.pagenationList(request.pageSize, request.PageNumber, result);
            int Totalcount = invoiceMasterRepositoryQuery.TableNoTracking
                .Where(q => q.BranchId == userInfo.CurrentbranchId &&
                          ((q.InvoiceTypeId == (int)DocumentType.OutgoingTransfer && q.IsDeleted == false) ||
                            q.InvoiceTypeId == (int)DocumentType.DeleteAddPermission)).Count();

            return new ResponseResult() { Data = FinalData, DataCount = result.Count(), TotalCount = Totalcount };
        }
    }
}
