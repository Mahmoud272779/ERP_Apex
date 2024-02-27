using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetItemsInPartsDropDown
{
    public class GetItemsInPartsDropDownHandler : IRequestHandler<GetItemsInPartsDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;

        public GetItemsInPartsDropDownHandler(IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository)
        {
            this.itemCardMasterRepository = itemCardMasterRepository;
        }

        public async Task<ResponseResult> Handle(GetItemsInPartsDropDownRequest request, CancellationToken cancellationToken)
        {
            var ItemsList = itemCardMasterRepository.TableNoTracking.Where(e => e.Status == (int)Status.Active && e.TypeId == (int)ItemTypes.Store).Select(a => new { a.Id, a.ItemCode, a.ArabicName, a.LatinName });

            return new ResponseResult() { Data = ItemsList, Id = null, Result = ItemsList.Any() ? Result.Success : Result.Failed };
        }
    }
}
