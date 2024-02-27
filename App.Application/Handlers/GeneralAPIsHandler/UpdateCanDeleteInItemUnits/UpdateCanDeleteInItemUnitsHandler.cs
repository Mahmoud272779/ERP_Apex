using App.Domain.Entities.Setup;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.UpdateCanDeleteInItemUnits
{
    public class UpdateCanDeleteInItemUnitsHandler : IRequestHandler<UpdateCanDeleteInItemUnitsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryCommand<InvStpItemCardUnit> itemUnitsCommand;

        public UpdateCanDeleteInItemUnitsHandler(IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery, IRepositoryCommand<InvStpItemCardUnit> itemUnitsCommand)
        {
            this.itemUnitsQuery = itemUnitsQuery;
            this.itemUnitsCommand = itemUnitsCommand;
        }

        public async Task<ResponseResult> Handle(UpdateCanDeleteInItemUnitsRequest request, CancellationToken cancellationToken)
        {
            var updatedItem = itemUnitsQuery.FindAll(a => a.ItemId == request.itemId && (request.unitId != null ? a.UnitId == request.unitId : 1 == 1));

            if (updatedItem.Count > 0)
            {
                foreach (var item in updatedItem)
                {
                    item.WillDelete = request.delete;
                    await itemUnitsCommand.UpdateAsyn(updatedItem);
                }
            }
            return new ResponseResult() { Data = null, Id = null, Result = updatedItem == null ? Result.Failed : Result.Success };
        }
    }
}
