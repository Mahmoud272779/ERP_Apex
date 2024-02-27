using MediatR;

namespace App.Application.Handlers.Categories
{

    public class GetCategoryHistoryRequest : IRequest<ResponseResult>
    {
        public int CategoryId { get; set; }
    }
}
