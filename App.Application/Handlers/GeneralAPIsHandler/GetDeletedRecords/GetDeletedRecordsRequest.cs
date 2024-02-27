using MediatR;

namespace App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecors
{
    public class GetDeletedRecordsRequest : IRequest<ResponseResult>
    {
        public DateTime date { get; set; }
    }
}
