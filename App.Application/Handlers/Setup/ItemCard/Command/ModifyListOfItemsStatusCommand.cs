using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Setup;
using App.Domain.Entities.Setup.Services;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{
    public class ModifyListOfItemsStatusCommand : BaseClass, IRequestHandler<ModifyListOfItemsStatusRequest, ResponseResult>

    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        private readonly IHistory<InvStpItemCardHistory> history;

        public ModifyListOfItemsStatusCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository, IHistory<InvStpItemCardHistory> history) : base(httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            itemCardRepository = ItemCardRepository;
            this.history  = history ;
        }
 
        public async Task<ResponseResult> Handle(ModifyListOfItemsStatusRequest request, CancellationToken cancellationToken)
        {
            var res = new ResponseResult();
            var items = await itemCardRepository.Get(e => request.Id.Contains(e.Id));
            var itemList = new List<InvStpItemCardMaster>();
            Mapping.Mapper.Map(items, itemList, typeof(ModifyListOfItemsStatusRequest), typeof(InvStpItemCardMaster));

            var shrinkedList = new List<GetAllItemsResponse>();
           
            itemList.Select(e => { e.Status = request.Status; return e; } ).ToList();
            if(request.Id.Contains(1))
            itemList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            
            
            var result = await itemCardRepository.UpdateAsyn(itemList);
            foreach (var item in itemList)
            {
                history.AddHistory(item.Id, item.LatinName, item.ArabicName, Aliases.HistoryActions.Update , Aliases.TemporaryRequiredData.UserName);

            }
            Mapping.Mapper.Map(itemList, shrinkedList, typeof(InvStpItemCardMaster), typeof(GetAllItemsResponse));
            res.Result = result ? Result.Success : Result.Failed;
            res.Data = shrinkedList;
            return res;
        }
    }
}
