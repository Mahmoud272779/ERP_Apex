using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{
    public class DeleteItemUnitCommand : BaseClass, IRequestHandler<DeleteItemUnitRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository;

        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        public DeleteItemUnitCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository, IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository) : base(httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            this.itemUnitRepository = itemUnitRepository;
        }

        public async Task<ResponseResult> Handle(DeleteItemUnitRequest request, CancellationToken cancellationToken)
        {
            if (request.ItemId==1)
            {
                return new ResponseResult() { Result =  Result.CanNotBeDeleted, Id = request.ItemId };
            }
            var res = await itemUnitRepository.DeleteAsync(e => e.ItemId == request.ItemId && e.UnitId == request.UnitId);
            var result = new ResponseResult() { Result = res ? Result.Success : Result.Failed, Id = request.ItemId };
            return result;
        }
    }
}
