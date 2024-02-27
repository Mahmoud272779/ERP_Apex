using MediatR;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesByIdRequest : IRequest<ResponseResult>
    {
        public int Id { get; set; }
    }
}
