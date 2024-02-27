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
    public class DeleteItemPartCommand : BaseClass, IRequestHandler<DeleteItemPartRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand;
        public DeleteItemPartCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemPartRepositoryCommand = itemPartRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(DeleteItemPartRequest request, CancellationToken cancellationToken)
        {
            var res = await itemPartRepositoryCommand.DeleteAsync(e => e.ItemId == request.ItemId && e.PartId == request.PartId);
            var result = new ResponseResult() { Result = res ? Result.Success : Result.Failed, Id = request.ItemId };
            return result;
        }
    }
}
