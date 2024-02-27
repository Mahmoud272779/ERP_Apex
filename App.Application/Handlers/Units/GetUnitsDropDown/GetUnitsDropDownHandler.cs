using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class GetUnitsDropDownHandler : IRequestHandler<GetUnitsDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;

        public GetUnitsDropDownHandler(IRepositoryQuery<InvStpUnits> unitsRepositoryQuery)
        {
            UnitsRepositoryQuery = unitsRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetUnitsDropDownRequest request, CancellationToken cancellationToken)
        {
            var dropdownlist = await UnitsRepositoryQuery.Get(e => new { e.Id, e.Code, e.ArabicName, e.LatinName, e.Status }, e => e.Status == (int)Status.Active);
            return new ResponseResult() { Data = dropdownlist, DataCount = dropdownlist.Count, Result = dropdownlist.Any() ? Result.Success : Result.Failed };
        }
    }
}
