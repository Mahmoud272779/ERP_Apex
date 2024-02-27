using MediatR;

namespace App.Application.Handlers.Restaurants
{
    public class UpdateStatusRequest : SharedRequestDTOs.UpdateStatus,IRequest<ResponseResult>
    {
    }
}
