using MediatR;

namespace App.Application.Handlers.Units
{
    public class UpdateUnitsRequest : UpdateUnitsParameter,IRequest<ResponseResult>
    {
    }
}
