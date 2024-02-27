using MediatR;

namespace App.Application.Handlers.Persons
{
    public class GetPersonHistoryRequest : IRequest<ResponseResult>
    {
        public int Code { get; set; }
    }
}
