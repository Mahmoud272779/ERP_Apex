using MediatR;

namespace App.Application.Handlers.Units
{
    public class DeleteKitchensRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
