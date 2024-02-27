using MediatR;

namespace App.Application.Handlers.Restaurants
{
    public class GetKitchensHistoryRequest : IRequest<ResponseResult>
    {
        public int Code { get; set; }
    }
}
