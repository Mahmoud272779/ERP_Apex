using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Persons.GetPersonsByDate
{
    public class GetPersonsByDateRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime date { get; set; }
    }
}
