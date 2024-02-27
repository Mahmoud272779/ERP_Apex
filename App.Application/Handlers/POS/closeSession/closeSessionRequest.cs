using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.closeSession
{
    public class closeSessionRequest : IRequest<ResponseResult>
    {
        public int sessionId { get; set; }
    }
}
