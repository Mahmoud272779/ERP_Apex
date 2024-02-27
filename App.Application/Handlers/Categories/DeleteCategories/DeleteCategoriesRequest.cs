using MediatR;

namespace App.Application.Handlers.Categories
{
    public class DeleteCategoriesRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
