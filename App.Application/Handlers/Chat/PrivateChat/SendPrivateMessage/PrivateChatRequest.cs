using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.PrivateChat.Chat
{
    public class PrivateChatRequest : IRequest<ResponseResult>
    {
        public int IdTo { get; set; }
        public string message { get; set; }
    }
}
