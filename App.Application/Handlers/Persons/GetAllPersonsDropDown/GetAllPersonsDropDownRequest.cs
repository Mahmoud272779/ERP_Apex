using App.Domain.Models.Common;
using MediatR;

namespace App.Application.Handlers.Persons
{
    public class GetAllPersonsDropDownRequest : DropDownRequestForPerson,IRequest<ResponseResult>
    {
    }
}
