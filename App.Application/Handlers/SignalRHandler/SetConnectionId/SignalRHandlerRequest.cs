using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.SignalRHandler
{
    public class SignalRHandlerRequest : IRequest<ResponseResult>
    {
        public string connectionId { get; set; }
    }
}
