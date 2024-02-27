using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Groups
{
    public class editGroupInfoRequest : IRequest<ResponseResult>
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public IFormFile image { get; set; }
        public bool allowReply { get; set; }
        public bool canExit { get; set; }
    }
}
