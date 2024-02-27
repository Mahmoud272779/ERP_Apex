using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetAllCategoriesDropDownHandler : IRequestHandler<GetAllCategoriesDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;

        public GetAllCategoriesDropDownHandler(IRepositoryQuery<InvCategories> categoriesRepositoryQuery)
        {
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetAllCategoriesDropDownRequest request, CancellationToken cancellationToken)
        {
            var CategoryList = CategoriesRepositoryQuery.TableNoTracking.Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName, a.Status });

            return new ResponseResult() { Data = CategoryList, Id = null, Result = CategoryList.Any() ? Result.Success : Result.Failed };
        }
    }
}
