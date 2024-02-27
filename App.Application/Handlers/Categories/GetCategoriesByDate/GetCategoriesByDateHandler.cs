using App.Application.Services.Process.Invoices.General_APIs;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System.Threading;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesByDateHandler : IRequestHandler<GetCategoriesByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCategories> CategoriesRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetCategoriesByDateHandler(IRepositoryQuery<InvCategories> categoriesRepositoryQuery, IGeneralAPIsService generalAPIsService)
        {
            CategoriesRepositoryQuery = categoriesRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
        }
        public async Task<ResponseResult> Handle(GetCategoriesByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await CategoriesRepositoryQuery.TableNoTracking.Where(q => q.UTime >= request.date).ToListAsync();

                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);


            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }

    }
}
