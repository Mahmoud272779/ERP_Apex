using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesDropDownHandler : IRequestHandler<GetCategoriesDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;

        public GetCategoriesDropDownHandler(IRepositoryQuery<InvCategories> categoriesRepositoryQuery)
        {
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetCategoriesDropDownRequest request, CancellationToken cancellationToken)
        {
            var CategoryList = CategoriesRepositoryQuery.TableNoTracking.Where(e => e.Status == (int)Status.Active
                            && (request.SearchCriteria != null ? (e.ArabicName.Contains(request.SearchCriteria) ||
                            e.LatinName.Contains(request.SearchCriteria)) : true))
                          .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName, a.Status,a.VatValue }).ToList();
            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                CategoryList = CategoryList.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }

            var totalCount = CategoriesRepositoryQuery.TableNoTracking.Where(e => e.Status == (int)Status.Active).Count();

            return new ResponseResult()
            {
                Data = CategoryList,
                DataCount = CategoryList.Count(),
                Id = null,
                Result = CategoryList.Any() ? Result.Success : Result.Failed
                        ,
                TotalCount = totalCount,
                Note = (CategoryList.Count() < request.PageSize ? Actions.EndOfData : "")
            };
        }
    }
}
