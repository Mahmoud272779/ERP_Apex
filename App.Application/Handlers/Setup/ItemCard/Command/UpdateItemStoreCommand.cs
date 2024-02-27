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
    public class UpdateItemStoreCommand : BaseClass, IRequestHandler<UpdateItemStoreRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardStores> itemSotreRepositoryCommand;
        public UpdateItemStoreCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardStores> itemSotreRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemSotreRepositoryCommand = itemSotreRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(UpdateItemStoreRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();

            var item = await itemSotreRepositoryCommand.GetByAsync(e => e.ItemId == request.Id && e.StoreId == request.StoreId);
            Mapping.Mapper.Map(request, item, typeof(UpdateItemStoreRequest), typeof(InvStpItemCardStores));
            var result = await itemSotreRepositoryCommand.UpdateAsyn(item);
            res.Result = result ? Result.Success : Result.Failed;
            res.Id = item.ItemId;
            res.Data = item;

            return res;
        }
    }
}
