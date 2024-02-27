using App.Application.Handlers.POS.AddPOSSessionHistory;
using App.Domain.Entities.POS;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.SessionBinding
{
    public class SessionBindingHandler : IRequestHandler<SessionBindingRequest, ResponseResult>
    {
        private readonly iUserInformation _userInformation;
        private readonly IRepositoryQuery<POSSession> _POSSessionQurey;
        private readonly IRepositoryCommand<POSSession> _POSSessionCommand;
        private readonly IMediator _mediator;

        public SessionBindingHandler(iUserInformation userInformation, IRepositoryQuery<POSSession> pOSSessionQurey, IRepositoryCommand<POSSession> pOSSessionCommand, IMediator mediator)
        {
            _userInformation = userInformation;
            _POSSessionQurey = pOSSessionQurey;
            _POSSessionCommand = pOSSessionCommand;
            _mediator = mediator;
        }

        public async Task<ResponseResult> Handle(SessionBindingRequest request, CancellationToken cancellationToken)
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
                    ErrorMessageAr = "لا يمكن تعليق جلسه مغلقه",
                    ErrorMessageEn = "You can not bind closed Session"
                };
            if (POSSessions.sessionStatus == (int)POSSessionStatus.bining)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن تعليق جلسه معلقه",
                    ErrorMessageEn = "You can not bind binded Session"
                };
            POSSessions.sessionStatus = (int)POSSessionStatus.bining;
            if (userInfo.otherSettings.allowCloseCloudPOSSession)
            {
                var updated = await _POSSessionCommand.UpdateAsyn(POSSessions);
                if (updated)
                    await _mediator.Send(new addPOSSessionHistoryRequest
                    {
                        POSSessionId = POSSessions.Id,
                        actionAr = "اغلاق الجلسة",
                        actionEn = "Close Session"
                    });
            }
            else
            {
                if (POSSessions.employeeId == userInfo.employeeId)
                {
                    var updated = await _POSSessionCommand.UpdateAsyn(POSSessions);
                    if (updated)
                        await _mediator.Send(new addPOSSessionHistoryRequest
                        {
                            POSSessionId = POSSessions.Id,
                            actionAr = "اغلاق الجلسة",
                            actionEn = "Close Session"
                        });
                    return new ResponseResult
                    {
                        Result = Result.Success
                    };
                }
                else
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = "لا يمكنك تعليق جلسه مستخدم اخر",
                        ErrorMessageEn = "You can not bind other user session"
                    };
            }

            return new ResponseResult{Result = Result.Success};
        }
    }
}
