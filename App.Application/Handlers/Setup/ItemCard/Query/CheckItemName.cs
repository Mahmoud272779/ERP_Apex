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
  public  class CheckItemName : IRequestHandler<CheckItemNameRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> ItemCardMasterQuery;


     
        public CheckItemName(IRepositoryQuery<InvStpItemCardMaster> ItemCardMasterQuery)
        {
            this.ItemCardMasterQuery = ItemCardMasterQuery;
         
        }


        public async Task<ResponseResult> Handle(CheckItemNameRequest request, CancellationToken cancellationToken)
        {

            var res = new ResponseResult();

            ICollection<InvStpItemCardMaster> itemExists = new List<InvStpItemCardMaster>();
          
            itemExists = await ItemCardMasterQuery.FindAllAsync(e => e.ArabicName == request.ItemName.Trim() && (request.ItemId>0 ? e.Id != request.ItemId : 1==1) );
           
            if (itemExists.Any())
                res.Result = Result.Exist;
            else
                res.Result = Result.NoDataFound;

            return res;

        }
    }
}
