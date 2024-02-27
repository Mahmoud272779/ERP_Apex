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
    public class UpdateItemPartCommand : BaseClass, IRequestHandler<UpdateItemPartRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand;
        public UpdateItemPartCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemPartRepositoryCommand = itemPartRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(UpdateItemPartRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();

            var item = await itemPartRepositoryCommand.GetByAsync(e => e.ItemId == request.Id && e.PartId == request.PartId);
            Mapping.Mapper.Map(request, item, typeof(UpdateItemPartRequest), typeof(InvStpItemCardParts));
            var result = await itemPartRepositoryCommand.UpdateAsyn(item);
            res.Result = result ? Result.Success : Result.Failed;
            res.Id = item.ItemId;
            res.Data = item;

            return res;
        }
    }
}
