using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.GetPOSSessionHistory
{
    public class getPOSSessionHistoryRequest:IRequest<ResponseResult>
    {
        public int sessionId { get; set; }
    }
}
