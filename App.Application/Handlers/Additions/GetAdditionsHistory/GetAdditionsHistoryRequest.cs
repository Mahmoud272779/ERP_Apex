using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Additions.GetAdditionsHistory
{
    public  class GetAdditionsHistoryRequest : IRequest<ResponseResult>
    {
        public int Code { get; set; }
    }
}
