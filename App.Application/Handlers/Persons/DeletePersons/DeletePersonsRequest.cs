using MediatR;

namespace App.Application.Handlers.Persons
{
    public class DeletePersonsRequest : SharedRequestDTOs.Delete,IRequest<ResponseResult>
    {
    }
}
