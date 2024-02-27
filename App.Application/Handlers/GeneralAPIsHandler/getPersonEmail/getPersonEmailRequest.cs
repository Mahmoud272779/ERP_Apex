using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.getPersonEmail
{
    public class getPersonEmailRequest : IRequest<ResponseResult>
    {
        public int personId { get; set; }
    }
}
