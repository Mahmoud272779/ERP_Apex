using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Transfer
{
    public class DeleteOutgoingTransferService : IRequestHandler<DeleteTransferRequest, ResponseResult>
    {
        private readonly IDeleteInvoice generalProcess;



        public DeleteOutgoingTransferService(
                              IDeleteInvoice generalProcess
                    )
        {
            this.generalProcess = generalProcess;
        }
        public async Task<ResponseResult> Handle(DeleteTransferRequest request, CancellationToken cancellationToken)
        {


            var res = await generalProcess.DeleteInvoices(request.Ids.First(), (int)DocumentType.DeletedOutgoingTransfer, InvoicesCode.DeletedOutgoingTransfer, (int)DocumentType.OutgoingTransfer);


            return res;
        }


        //throw new NotImplementedException();
    }
}

