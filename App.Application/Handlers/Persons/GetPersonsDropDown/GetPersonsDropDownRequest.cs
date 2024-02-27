using App.Domain.Models.Common;
using MediatR;

namespace App.Application.Handlers.Persons
{
    public class GetPersonsDropDownRequest : DropDownRequestForPerson, IRequest<ResponseResult>
    {
    }
}
