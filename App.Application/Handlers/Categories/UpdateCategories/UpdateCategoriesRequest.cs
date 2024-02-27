using MediatR;

namespace App.Application.Handlers.Categories
{
    public class UpdateCategoriesRequest : UpdateCategoryParameter,IRequest<ResponseResult>
    {
    }
}
