using MediatR;

namespace App.Application.Handlers.Categories
{
    public class AddCategoryRequest : CategoriesParameter,IRequest<ResponseResult>
    {
    }
}
