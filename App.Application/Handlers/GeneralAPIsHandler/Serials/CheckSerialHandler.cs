using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.Serials
{
    internal class CheckSerialHandler : IRequestHandler<CheckSerialRequest, serialsReponse>
    {
        private readonly IRepositoryQuery<InvSerialTransaction> serialTransactionQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        public CheckSerialHandler(IRepositoryQuery<InvSerialTransaction> serialTransactionQuery, IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.serialTransactionQuery = serialTransactionQuery;
            this.itemCardMasterRepository = itemCardMasterRepository;
            this.itemUnitsQuery = itemUnitsQuery;
        }


        public async Task<serialsReponse> Handle(CheckSerialRequest request, CancellationToken cancellationToken)
        {
            return CheckSerialMethod.CheckSerial(new serialsRequest
            {
                endPattern = request.endPattern,
                fromNumber = request.fromNumber,
                invoiceType = request.invoiceType,
                isDiffNumbers = request.isDiffNumbers,
                newEnteredSerials = request.newEnteredSerials,
                serial = request.serial,
                serialsInTheSameInvoice = request.serialsInTheSameInvoice,
                stratPattern = request.stratPattern,
                toNumber = request.toNumber,
                serialRemovedInEdit=request.serialRemovedInEdit,
            }, false, serialTransactionQuery, itemCardMasterRepository, itemUnitsQuery);
        }
    }
}
