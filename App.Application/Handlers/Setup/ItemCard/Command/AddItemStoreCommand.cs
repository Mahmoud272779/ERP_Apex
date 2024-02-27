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
    public class AddItemStoreCommand : BaseClass, IRequestHandler<AddItemStoreRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardStores> itemSotreRepositoryCommand;
        public AddItemStoreCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardStores> itemSotreRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemSotreRepositoryCommand = itemSotreRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(AddItemStoreRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();
            var itemExists = await itemSotreRepositoryCommand.GetByAsync(e => e.ItemId == request.ItemId && e.StoreId == request.StoreId);
            if (itemExists != null)
            {
                res.Result = Result.Exist;
            }
            else
            {
                var item = new InvStpItemCardStores();

                Mapping.Mapper.Map(request, item, typeof(AddItemStoreRequest), typeof(InvStpItemCardStores));
                var result = await itemSotreRepositoryCommand.AddAsync(item);
                res.Result = result ? Result.Success : Result.Failed;
                res.Id = item.ItemId;
                res.Data = item;
            }
            return res;
        }
    }
}
