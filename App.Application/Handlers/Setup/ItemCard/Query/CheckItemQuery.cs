using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class CheckItemQuery : IRequestHandler<CheckItemRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery;

        public CheckItemQuery(IRepositoryQuery<InvStpItemCardMaster> ItemCardRepositoryQuery)
        {
            itemCardRepositoryQuery = ItemCardRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(CheckItemRequest request, CancellationToken cancellationToken)
        {
            var itemCard = await itemCardRepositoryQuery.FindAsync(e => e.ItemCode == request.Code);
            if (itemCard == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };

            }
            else if (itemCard.Status == 2)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.InActive };
            }
            else
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
            }
        }
    }
}
