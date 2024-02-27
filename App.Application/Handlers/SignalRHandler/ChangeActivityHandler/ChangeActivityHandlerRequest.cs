using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.SignalRHandler.ChangeActivityHandler
{
    public class ChangeActivityHandlerRequest : IRequest<ResponseResult>
    {
        public bool isActive { get; set; }
        public string connectionId { get; set; }
    }
}
