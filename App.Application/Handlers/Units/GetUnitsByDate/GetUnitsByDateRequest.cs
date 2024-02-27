using MediatR;

namespace App.Application.Handlers.Units
{
    public class GetUnitsByDateRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime date { get; set; }
    }
}
