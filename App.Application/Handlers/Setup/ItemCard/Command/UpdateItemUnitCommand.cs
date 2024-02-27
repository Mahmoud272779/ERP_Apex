using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
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
    public class UpdateItemUnitCommand : BaseClass, IRequestHandler<UpdateItemUnitRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository;

        public UpdateItemUnitCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository, IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository) : base(httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            this.itemUnitRepository = itemUnitRepository;
        }
        public async Task<ResponseResult> Handle(UpdateItemUnitRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();
            
                var item = await itemUnitRepository.GetByAsync(e => e.ItemId == request.Id && e.UnitId ==request.UnitId);
                Mapping.Mapper.Map(request, item, typeof(UpdateItemUnitRequest), typeof(InvStpItemCardUnit));
                var result = await itemUnitRepository.UpdateAsyn(item);
                res.Result = result ? Result.Success : Result.Failed;
                res.Id = item.ItemId;
                res.Data = item;
            
            return res;
        }
    }
}
