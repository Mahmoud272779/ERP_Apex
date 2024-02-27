using MediatR;

namespace App.Application.Handlers.Persons
{
    public class GetListOfPersonsRequest : PersonsSearch, IRequest<ResponseResult>
    {
        public string ids { get; set; }
        public bool isSearchData { get; set; } = true;
        public bool isPrint { get; set; } = false;
    }
}
