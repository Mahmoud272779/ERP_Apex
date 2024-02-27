using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Groups
{
    public class sendGroupMessageRequest : IRequest<ResponseResult>
    {
        public int groupId { get; set; }
        public string message { get; set; }
    }
}
