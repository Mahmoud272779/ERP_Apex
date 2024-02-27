using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount
{
    public class GetNotificationNotSeenCountRequest : IRequest<int>
    {
        public string companyLogin { get; set; }
        public int employeeId { get; set; } 
    }
}
