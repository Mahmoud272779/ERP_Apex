using App.Domain.Entities.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.NotificationSeen
{
    public class NotificationSeenHandler : IRequestHandler<NotificationSeenRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<App.Domain.Entities.Notification.NotificationSeen> _NotificationSeenCommand;
        private readonly iUserInformation _iUserInformation;
        public NotificationSeenHandler(IRepositoryCommand<Domain.Entities.Notification.NotificationSeen> notificationSeenCommand, iUserInformation iUserInformation)
        {
            _NotificationSeenCommand = notificationSeenCommand;
            _iUserInformation = iUserInformation;
        }
        public async Task<ResponseResult> Handle(NotificationSeenRequest request, CancellationToken cancellationToken)
        {

            if (request.Id <= 0)
                return new ResponseResult()
                {
                    Result = Result.Success 
                };
            var userinfo = await _iUserInformation.GetUserInformation();

            var obj = new Domain.Entities.Notification.NotificationSeen
            {
                NotificationsMasterId = request.Id,
                UserId = userinfo.employeeId,
                isAdmin = request.notificationType == (int)notificationTypes.admin ? true : false
            };

            _NotificationSeenCommand.Add(obj);
            var inserted = await _NotificationSeenCommand.SaveChanges() > 0 ? true:false;
            return new ResponseResult()
            {
                Result = inserted ? Result.Success : Result.Failed
            };
        }
    }
}
