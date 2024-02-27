using MediatR;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesByDateRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime date { get; set; }
    }
}
