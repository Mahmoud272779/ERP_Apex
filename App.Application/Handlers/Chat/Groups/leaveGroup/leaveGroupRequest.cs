using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Groups
{
    public class leaveGroupRequest : IRequest<ResponseResult>
    {
        public int groupId { get; set; }
    }
}
