using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Entities.Setup.Services;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{
    public class DeleteItemCommand : BaseClass, IRequestHandler<DeleteItemRequest, ResponseResult>, IInvStpItemCardCommandHistory
    {
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryCommand<InvStpItemCardHistory> itemCardHistoryCommand; 
        private readonly IRepositoryQuery<InvSerialTransaction> Serialrepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> PartsQuery;
        private readonly IRepositoryCommand<InvStpItemCardParts> PartsCommand;

        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsrepository;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public DeleteItemCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository,
            IRepositoryCommand<InvStpItemCardHistory> itemCardHistoryCommand
            ,  IRepositoryQuery<InvStpItemCardMaster> ItemCardQueryRepository,
            IRepositoryQuery<InvoiceDetails> invoiceDetailsrepository,
            IRepositoryQuery<InvStpItemCardStores> storesrepository,
            ISystemHistoryLogsService systemHistoryLogsService,
            IRepositoryQuery<InvSerialTransaction> serialrepository,
            IRepositoryQuery<InvStpItemCardParts> Partsrepository,
            IRepositoryCommand<InvStpItemCardParts> PartsCommand) : base(httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            Serialrepository = serialrepository;
            InvoiceDetailsrepository = invoiceDetailsrepository;
            _systemHistoryLogsService = systemHistoryLogsService;
            this.itemCardHistoryCommand = itemCardHistoryCommand;
            this.PartsQuery = Partsrepository;
            this.PartsCommand = PartsCommand;
        }

        public void AddHistory(InvStpItemCardHistory invStpItemCardHistory)
        {
            itemCardHistoryCommand.AddAsync(invStpItemCardHistory);
        }

        public async Task<ResponseResult> Handle(DeleteItemRequest request, CancellationToken cancellationToken)
        {
           
            var serial = Serialrepository.FindAll(e => e.ItemId == request.ItemId);
           
            var invoiceDetails = InvoiceDetailsrepository.FindAll(q=>q.ItemId==request.ItemId);
            if (request.ItemId == 1|| serial.Count() > 0||invoiceDetails.Count()>0)
            {
                return new ResponseResult() { Result = Result.CanNotBeDeleted, Id = request.ItemId ,Note="Can not be deleted"};
            }
            var parts = PartsQuery.FindAll(a => a.PartId == request.ItemId);
            if(parts.Count>0)
              await PartsCommand.DeleteAsync(e => e.PartId == request.ItemId);
            var res = await itemCardRepository.DeleteAsync(e => e.Id == request.ItemId);

            if (res)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteItemCard);
            var result = new ResponseResult() { Result = res ? Result.Success : Result.Failed, Id = request.ItemId };
            return result;  
        }
    }
}
