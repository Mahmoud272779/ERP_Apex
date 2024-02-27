using MediatR;

namespace App.Application.Handlers.Persons
{
    public class UpdatePersonsRequest : UpdatePersonRequest,IRequest<ResponseResult>
    {
    }
}
