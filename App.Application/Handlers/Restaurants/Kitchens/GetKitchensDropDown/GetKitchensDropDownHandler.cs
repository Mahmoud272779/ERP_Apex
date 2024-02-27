using App.Application.Handlers.Restaurants;
using App.Domain.Entities.Process.Restaurants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class GetKitchensDropDownHandler : IRequestHandler<GetKitchensDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<Kitchens> kitchenQuery;

        public GetKitchensDropDownHandler(IRepositoryQuery<Kitchens> kitchenQuery)
        {
            this.kitchenQuery = kitchenQuery;
        }
        public async Task<ResponseResult> Handle(GetKitchensDropDownRequest request, CancellationToken cancellationToken)
        {
            var dropdownlist = await kitchenQuery.Get(e => new { e.Id, e.Code, e.ArabicName, e.LatinName, e.Status }, e => e.Status == (int)Status.Active);
            return new ResponseResult() { Data = dropdownlist, DataCount = dropdownlist.Count, Result = dropdownlist.Any() ? Result.Success : Result.Failed };
        }
    }
}
