using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetItemUnitsDropDown
{
    public class GetItemUnitsDropDownHandler : IRequestHandler<GetItemUnitsDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;

        public GetItemUnitsDropDownHandler(IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.itemUnitsQuery = itemUnitsQuery;
        }

        public async Task<ResponseResult> Handle(GetItemUnitsDropDownRequest request, CancellationToken cancellationToken)
        {
            var UnitsList = itemUnitsQuery.TableNoTracking.Where(e => e.ItemId == request.itemId)
               .Include(a => a.Unit).Select(a => new { a.UnitId, a.Unit.Code, a.Unit.ArabicName, a.Unit.LatinName, a.Barcode })
            .OrderByDescending(a => (string.IsNullOrEmpty(request.barcode) ? 1 == 1 : a.Barcode == request.barcode.ToUpper())).ThenBy(a => a.UnitId);

            return new ResponseResult() { Data = UnitsList, Id = null, Result = UnitsList.Any() ? Result.Success : Result.Failed };
        }
    }
}
