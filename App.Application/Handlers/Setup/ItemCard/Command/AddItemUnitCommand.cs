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
    public class AddItemUnitCommand : BaseClass, IRequestHandler<AddItemUnitRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository;
        public AddItemUnitCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository, IRepositoryCommand<InvStpItemCardUnit> itemUnitRepository) : base(httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            this.itemUnitRepository = itemUnitRepository;
        }

        public async Task<ResponseResult> Handle(AddItemUnitRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();
            var itemExists = await itemUnitRepository.GetByAsync(e => e.ItemId == request.ItemId && e.UnitId == request.UnitId);
            if (itemExists != null)
            {
                res.Result = Result.Exist;
            }
            else
            {
                var item = new InvStpItemCardUnit();
                
                Mapping.Mapper.Map(request, item, typeof(AddItemUnitRequest), typeof(InvStpItemCardUnit));
                var result = await itemUnitRepository.AddAsync(item);
                res.Result = result ? Result.Success : Result.Failed;
                res.Id = item.ItemId;
                res.Data = item;
            }
            return res;
        }
    }
}
