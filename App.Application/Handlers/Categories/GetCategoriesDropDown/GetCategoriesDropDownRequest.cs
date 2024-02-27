using App.Domain.Models.Common;
using MediatR;

namespace App.Application.Handlers.Categories
{
    public class GetCategoriesDropDownRequest : DropDownRequest,IRequest<ResponseResult>
    {
    }
}
