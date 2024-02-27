using MediatR;

namespace App.Application.Handlers.Restaurants
{
    public class DeleteKitchensRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
