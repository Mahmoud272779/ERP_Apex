using App.Application.Handlers.Transfer.OutgoingTransfer.Command;
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
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
    public class GetByTransferOutgoing : IRequestHandler<getByIdTransferRequest, ResponseResult>
    {
        readonly IGetInvoiceByIdService generalProcess;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceMaster> invoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly iUserInformation userinformation;

        public GetByTransferOutgoing(
                              IGetInvoiceByIdService generalProcess,
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

        public async Task<ResponseResult> Handle(getByIdTransferRequest request, CancellationToken cancellationToken)
        {
            //UserInformationModel userInfo = userinformation.GetUserInformation();
            //var result = invoiceMasterRepositoryQuery.TableNoTracking.Where(h => h.InvoiceId == request.Id && h.storeTo!=null)


                //.Select(h => new
                //{
                //    InvoiceId = h.InvoiceId,
                //    BookIndex = h.BookIndex,
                //    Date = h.InvoiceDate,
                //    TransStatus = h.transferStatus,
                //    StoreToAR = h.storeTo.ArabicName,
                //    StoreToEN = h.storeTo.LatinName,
                //    StoreFromAR = h.store.ArabicName,
                //    StoreFromEN = h.store.LatinName,
                //    StoreFrom = h.StoreId,
                //    StoreTo = h.StoreId,
                //    h.InvoicesDetails
                //});

            var result = await generalProcess.GetInvoiceDto(request.Id, false,true,false);
            if ( result !=null && Lists.transferStore.Contains(result.InvoiceTypeId) && result.StoreIdTo == null )
            {
                return new ResponseResult() { Data = null,Result=Result.NoDataFound };
            }
          
            return new ResponseResult() { Data = result,Result= result==null? Result.NotFound: Result.Success };
        }
    }
}
