using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Transfer
{
    public class OutgoingTransferService : IRequestHandler<UpdateOutgoingTransferRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery;
        private readonly IUpdateInvoice generalProcess;
        private SettingsOfInvoice SettingsOfInvoice;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public OutgoingTransferService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IUpdateInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                               IGeneralAPIsService generalAPIsService)
        {
            invoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            this.generalProcess = generalProcess;
            invGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
        }
        public async Task<ResponseResult> Handle(UpdateOutgoingTransferRequest request, CancellationToken cancellationToken)
        {

            #region
            request.InvoiceDetails = request.InvoiceDetails.Where(a => !Lists.itemThatNotTransfer.Contains(a.ItemTypeId)).ToList();
            #endregion

            var setting = await invGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);


            var res = await generalProcess.UpdateInvoices(request, setting, null, (int)DocumentType.OutgoingTransfer, InvoicesCode.OutgoingTransfer, FilesDirectories.Sales);

            return res;
        }


        //throw new NotImplementedException();
    }
}

