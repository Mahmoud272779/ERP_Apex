using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Chat.Broadcast
{
    public class broadcastRequest : IRequest<ResponseResult>
    {
        public string companiesId { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string SecurityKey { get; set; }
        public bool AllCompanies { get; set; }
    }
}
