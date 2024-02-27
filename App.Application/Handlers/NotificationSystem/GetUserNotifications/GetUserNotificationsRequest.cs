using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.GetUserNotifications
{
    public class GetUserNotificationsRequest :PaginationVM, IRequest<ResponseResult>
    {
    }
}
