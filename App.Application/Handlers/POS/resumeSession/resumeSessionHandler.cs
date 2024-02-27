using App.Domain.Entities.POS;
using App.Domain.Models.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.resumeSession
{
    public class resumeSessionHandler : IRequestHandler<resumeSessionRequest, ResponseResult>
    {
        private readonly iUserInformation _userInformation;
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly IRepositoryCommand<POSSession> _POSSessionCommand;

        public resumeSessionHandler(iUserInformation userInformation, IRepositoryQuery<POSSession> pOSSessionQuery, IRepositoryCommand<POSSession> pOSSessionCommand)
        {
            _userInformation = userInformation;
            _POSSessionQuery = pOSSessionQuery;
            _POSSessionCommand = pOSSessionCommand;
        }

        public async Task<ResponseResult> Handle(resumeSessionRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _userInformation.GetUserInformation();
            var usreSession = _POSSessionQuery.TableNoTracking.Where(x => x.Id == request.sessionId).FirstOrDefault();
            if (usreSession == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "هذه الجلسه غير موجوده",
                    ErrorMessageEn = "This Session is Not exist"
                };
            if(usreSession.sessionStatus == (int)POSSessionStatus.closed)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن استكمال جلسه مغلقه",
                    ErrorMessageEn = "You can not resume closed session"
                };
            if (usreSession.sessionStatus == (int)POSSessionStatus.active)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن استكمال جلسه مفتوحة",
                    ErrorMessageEn = "You can not resume active session"
                };
            if(usreSession.employeeId != userInfo.employeeId)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن استكمال جلسه مستخدم اخر",
                    ErrorMessageEn = "You can not resume another user session"
                };
            usreSession.sessionStatus = (int)POSSessionStatus.active;
            var saved = await _POSSessionCommand.UpdateAsyn(usreSession);

            return new ResponseResult
            {
                Result = saved ? Result.Success : Result.Failed
            };
        }
    }
}
