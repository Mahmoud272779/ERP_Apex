using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Transfer
{
    public class AddOutgoingTransferService : IRequestHandler<AddOutgoingTransferRequest, ResponseResult>
    {

        private readonly IAddInvoice generalProcess;

        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;


        public AddOutgoingTransferService(
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery
                           )
        {

            this.generalProcess = generalProcess;
            invGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;

        }
        public async Task<ResponseResult> Handle(AddOutgoingTransferRequest request, CancellationToken cancellationToken)
        {


            #region



            request.ParentInvoiceCode = "";
            request.InvoiceDetails = request.InvoiceDetails.Where(a => !Lists.itemThatNotTransfer.Contains(a.ItemTypeId)).ToList();
            request.transferStatus = TransferStatus.Binded;

            #endregion

            var setting = await invGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);


            var res = await generalProcess.SaveInvoice(request, setting, null, (int)DocumentType.OutgoingTransfer, InvoicesCode.OutgoingTransfer, 0, FilesDirectories.Sales);

            return res;
        }


        //throw new NotImplementedException();
    }
}

