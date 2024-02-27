using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesByIdHandler : IRequestHandler<GetCategoriesByIdRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;

        public GetCategoriesByIdHandler(IRepositoryQuery<InvCategories> categoriesRepositoryQuery)
        {
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(GetCategoriesByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var treeData = await CategoriesRepositoryQuery.SingleOrDefault(q => q.Id == request.Id);
                return new ResponseResult() { Data = treeData, Id = null, Result = Result.Success };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
