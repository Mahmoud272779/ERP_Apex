using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.SessionBinding
{
    public class SessionBindingRequest : IRequest<ResponseResult>
    {
        public int sessionId { get; set; }

    }
}
