using MediatR;

namespace App.Application.Handlers.Units
{
    public class UpdateStatusRequest : SharedRequestDTOs.UpdateStatus,IRequest<ResponseResult>
    {
    }
}
