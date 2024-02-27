using MediatR;

namespace App.Application.Handlers.Units
{
    public class AddUnitRequest : UnitsParameter,IRequest<ResponseResult>
    {
    }
}
