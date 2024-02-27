using App.Application.Handlers.NotificationSystem.GetNotificationNotSeenCount;
using App.Application.Handlers.POS.AddPOSSessionHistory;
using App.Application.SignalRHub;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.General;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace App.Application.Handlers.POS.closeSession
{
    public class closeSessionHandler : IRequestHandler<closeSessionRequest, ResponseResult>
    {
        private readonly iUserInformation _userInformation;
        private readonly IRepositoryQuery<POSSession> _POSSessionQurey;
        private readonly IRepositoryQuery<signalR> _signalRQurey;
        private readonly IRepositoryCommand<POSSession> _POSSessionCommand;
        private readonly IMediator _mediator;
        private readonly IHubContext<NotificationHub> _hubContext;
        public closeSessionHandler(iUserInformation userInformation, IRepositoryQuery<POSSession> pOSSessionQurey, IRepositoryCommand<POSSession> pOSSessionCommand, IMediator mediator, IRepositoryQuery<signalR> signalRQurey, IHubContext<NotificationHub> hubContext)
        {
            _userInformation = userInformation;
            _POSSessionQurey = pOSSessionQurey;
            _POSSessionCommand = pOSSessionCommand;
            _mediator = mediator;
            _signalRQurey = signalRQurey;
            _hubContext = hubContext;
        }
        public async Task<ResponseResult> Handle(closeSessionRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _userInformation.GetUserInformation();
            var POSSessions = _POSSessionQurey.TableNoTracking.Where(x => x.Id == request.sessionId).FirstOrDefault();
            if (POSSessions == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "الجلسه غير موجوده",
                    ErrorMessageEn = "Session Is not exist"
                };
            if (POSSessions.sessionStatus == (int)POSSessionStatus.closed)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن غلق جلسه مغلقه",
                    ErrorMessageEn = "You can not bind closed Session"
                };
            POSSessions.sessionClosedById = userInfo.employeeId;
            POSSessions.end = DateTime.Now;
            POSSessions.sessionStatus = (int)POSSessionStatus.closed;
            if (userInfo.otherSettings.allowCloseCloudPOSSession)
            {
                var updated = await _POSSessionCommand.UpdateAsyn(POSSessions);
                if(updated)
                    await _mediator.Send(new addPOSSessionHistoryRequest
                    {
                        POSSessionId = POSSessions.Id,
                        actionAr = "انهاء الجلسة",
                        actionEn = "Close Session"
                    });
                var employeeHubConnectionId = _signalRQurey.TableNoTracking.Where(c => c.InvEmployeesId == POSSessions.employeeId).FirstOrDefault().connectionId;
                await _hubContext.Clients.Client(employeeHubConnectionId).SendAsync(defultData.Notification,new ResponseSignalR
                {
                    title = "اغلاق جلسه نقاط بيع",
                    desc = $"تم اغلاق جلسه نقاط البيع الخاصه بك من قبل {userInfo.employeeNameAr}",
                    titleEn = "Close POS Session",
                    descEn = $"Your POS Session has been closed by {userInfo.employeeNameEn}",
                    notificationNotSeenCount = await _mediator.Send(new GetNotificationNotSeenCountRequest { companyLogin = userInfo.companyLogin,employeeId = POSSessions.employeeId}),

                });
            }
            else
            {
                if (POSSessions.employeeId == userInfo.employeeId)
                    await _POSSessionCommand.UpdateAsyn(POSSessions);
                else
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "لا يمكنك تعليق جلسه مستخدم اخر",
                        ErrorMessageEn = "You can not bind other user session"
                    };
            }

            return null;
        }
    }
}
