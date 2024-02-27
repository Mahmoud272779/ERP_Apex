using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class GetAllPersonsDropDownHandler : IRequestHandler<GetAllPersonsDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IRepositoryQuery<InvSalesMan> _salesmanQuery;

        public GetAllPersonsDropDownHandler(IRepositoryQuery<InvSalesMan> salesmanQuery, IRepositoryQuery<InvPersons> personQuery)
        {
            _salesmanQuery = salesmanQuery;
            PersonQuery = personQuery;
        }

        public async Task<ResponseResult> Handle(GetAllPersonsDropDownRequest request, CancellationToken cancellationToken)
        {
            ResponseResult responseResult = new ResponseResult();
            await Task.Run(() =>
            {
                var _data = PersonQuery.TableNoTracking;

                var salesMan = _salesmanQuery.TableNoTracking.Select(x => new { x.Id, x.ArabicName, x.LatinName });

                var data = _data.Where(e => e.IsSupplier == request.IsSupplier)
                                .Where(e => !string.IsNullOrEmpty(request.SearchCriteria) ? (e.ArabicName.Contains(request.SearchCriteria) || e.LatinName.Contains(request.SearchCriteria) ): true).ToList();

                
                double MaxPageNumber = data.Count() / Convert.ToDouble(request.PageSize);
                var countofFilter = Math.Ceiling(MaxPageNumber);
                responseResult.DataCount = data.Count();
                if (request.PageSize > 0 && request.PageNumber > 0)
                {
                    data = data.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                }

                var totalCount = PersonQuery.TableNoTracking.Where(e => e.Status == (int)Status.Active).Count();


                var response = data
                 .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName, a.Status, a.Phone, a.SalesManId, salesman = (request.isHaveSalesman ? salesMan.Where(d => d.Id == a.SalesManId) : null) }).ToList();


                responseResult.Data = response;

                responseResult.Result = response.Any() ? Result.Success : Result.Failed;
                responseResult.TotalCount = totalCount;
                responseResult.Note = (countofFilter == request.PageNumber ? Actions.EndOfData : "");


            });
            return responseResult;
        }
    }
}
