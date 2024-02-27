using MediatR;

namespace App.Application.Handlers.Categories
{
    public class UpdateStatusRequest : UpdateActiveCategoriesParameter,IRequest<ResponseResult>
    {
    }
}
