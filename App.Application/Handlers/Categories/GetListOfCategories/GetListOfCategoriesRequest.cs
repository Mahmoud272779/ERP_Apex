using MediatR;

namespace App.Application.Handlers.Categories
{
    public class GetListOfCategoriesRequest : CategoriesSearch,IRequest<ResponseResult>
    {
    }
}
