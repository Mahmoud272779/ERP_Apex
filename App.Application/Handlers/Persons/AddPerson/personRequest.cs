using MediatR;

namespace App.Application.Handlers.Persons
{
    public class personRequest : PersonRequest,IRequest<ResponseResult>
    {
    }
 
}
