using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.NotificationSeen
{
    public class NotificationSeenRequest : IRequest<ResponseResult>
    {
        public int Id { get; set; }
        public int notificationType { get; set; }
    }
}
