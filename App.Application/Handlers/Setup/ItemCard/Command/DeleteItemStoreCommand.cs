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
    public class DeleteItemStoreCommand : BaseClass, IRequestHandler<DeleteItemStoreRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardStores> itemStoreRepositoryCommand;
        public DeleteItemStoreCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardStores> itemStoreRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemStoreRepositoryCommand = itemStoreRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(DeleteItemStoreRequest request, CancellationToken cancellationToken)
        {
            var res = await itemStoreRepositoryCommand.DeleteAsync(e => e.ItemId == request.ItemId && e.StoreId == request.StoreId);
            var result = new ResponseResult() { Result = res ? Result.Success : Result.Failed, Id = request.ItemId };
            return result;
        }
    }
}
