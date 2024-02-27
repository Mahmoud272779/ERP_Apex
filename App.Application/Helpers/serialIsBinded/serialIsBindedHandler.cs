using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Models.Request.Store.Reports.Purchases;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.serialIsBinded
{
    public class serialIsBindedHandler : IRequestHandler<serialIsBindedRequest, List<string>>
    {
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;

        public serialIsBindedHandler(IRepositoryQuery<InvSerialTransaction> serialTransactionQuery)
        {
            this.serialTransactionQuery = serialTransactionQuery;
        }

        public async Task<List<string>> Handle(serialIsBindedRequest request, CancellationToken cancellationToken)
        {
            // get binded serials 
            var serialsBinded = serialTransactionQuery.TableNoTracking
                             .Where(a => a.TransferStatus == TransferStatus.Binded &&
                             a.ExtractInvoice != request.invoiceType &&
                             (request.serialsRequest != null ? request.serialsRequest.Contains(a.SerialNumber) : a.SerialNumber == request.itemCode)).Select(a => a.SerialNumber).ToList();
            return serialsBinded;
        }
    }
}
