using App.Domain.Entities.Setup;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class GetListOfUnitsHandler : IRequestHandler<GetListOfUnitsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> InvStpItemCardUnitQuery;

        public GetListOfUnitsHandler(IRepositoryQuery<InvStpUnits> unitsRepositoryQuery, IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery)
        {
            UnitsRepositoryQuery = unitsRepositoryQuery;
            InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
        }

        public async Task<ResponseResult> Handle(GetListOfUnitsRequest parameters, CancellationToken cancellationToken)
        {
            var InvStpItemCardUnit = InvStpItemCardUnitQuery.TableNoTracking;
            var resData = UnitsRepositoryQuery.TableNoTracking
            .Where(x => !string.IsNullOrEmpty(parameters.Name) ? x.ArabicName.Contains(parameters.Name) || x.LatinName.Contains(parameters.Name) || x.Code.ToString().Contains(parameters.Name) : true)
            .Where(x => parameters.Status != 0 ? x.Status == parameters.Status : true);
            if (string.IsNullOrEmpty(parameters.Name))
                resData = resData.OrderByDescending(x => x.Code);
            else
                resData = resData.OrderByDescending(x => x.Code);
            var count = resData.Count();

            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            resData.Where(a => a.CardUnits.Count() == 0 && a.Id != 1);
            List<GetAllUnitsDTO> unitsList = new List<GetAllUnitsDTO>();
            Mapping.Mapper.Map(resData, unitsList);
            unitsList.ForEach(x => x.CanDelete = unitsHelper.isCanDeleteUnits(InvStpItemCardUnit, x.Id));
            return new ResponseResult() { Data = unitsList, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };
        }
    }
}
