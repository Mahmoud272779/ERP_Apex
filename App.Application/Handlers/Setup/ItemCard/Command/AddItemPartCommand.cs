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
    public class AddItemPartCommand : BaseClass, IRequestHandler<AddItemPartRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand;
        public AddItemPartCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardParts> itemPartRepositoryCommand) : base(httpContextAccessor)
        {
            this.itemPartRepositoryCommand = itemPartRepositoryCommand;
        }
        public async Task<ResponseResult> Handle(AddItemPartRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();
            var itemExists = await itemPartRepositoryCommand.GetByAsync(e => e.ItemId == request.ItemId && e.PartId == request.PartId);
            if (itemExists != null)
            {
                res.Result = Result.Exist;
            }
            else
            {
                var item = new InvStpItemCardParts();
                Mapping.Mapper.Map(request, item, typeof(AddItemPartRequest), typeof(InvStpItemCardParts));
                var result = await itemPartRepositoryCommand.AddAsync(item);
                res.Result = result ? Result.Success : Result.Failed; ;
                res.Data = item;
            }
            return res;
        }
    }
}
