using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Groups
{
    public class addGroupMembersRequest : IRequest<ResponseResult>
    {
        public int groupId { get; set; }
        public string employeesIds { get; set; }
    }
}
