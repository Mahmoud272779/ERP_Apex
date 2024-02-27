using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem
{
    public class ReportNotificationsRequest : IRequest<bool>
    {
        public string connectionId { get; set; }
        public string token { get; set; }
    }
}
