using MediatR;

namespace App.Application.Handlers.Units
{
    public class GetKitchensHistoryRequest : IRequest<ResponseResult>
    {
        public int Code { get; set; }
    }
}
