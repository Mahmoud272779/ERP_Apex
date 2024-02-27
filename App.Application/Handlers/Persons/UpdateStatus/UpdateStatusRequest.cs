using MediatR;

namespace App.Application.Handlers.Persons
{
    public class UpdateStatusRequest : SharedRequestDTOs.UpdateStatus,IRequest<ResponseResult>
    {
    }
}
