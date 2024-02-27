using MediatR;

namespace App.Application.Handlers.Units
{
    public class GetListOfUnitsRequest : UnitsSearch,IRequest<ResponseResult>
    {
    }
}
