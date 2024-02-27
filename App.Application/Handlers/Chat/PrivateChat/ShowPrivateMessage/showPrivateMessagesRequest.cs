using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.PrivateChat.ShowPrivateMessage
{
    public class showPrivateMessagesRequest : IRequest<ResponseResult>
    {
        public int messagesFromId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
