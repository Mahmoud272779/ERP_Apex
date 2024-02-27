using App.Infrastructure.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Login.ResumeSessionService
{
    public class ResumeSessionHandler : IRequestHandler<ResumeSessionRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryCommand<signinLogs> _signinLogsCommand;
        private readonly IRepositoryQuery<signinLogs> _signinLogsQuery;
        public ResumeSessionHandler(iUserInformation iUserInformation, IRepositoryCommand<signinLogs> signinLogsCommand, IRepositoryQuery<signinLogs> signinLogsQuery)
        {
            _iUserInformation = iUserInformation;
            _signinLogsCommand = signinLogsCommand;
            _signinLogsQuery = signinLogsQuery;
        }
        public async Task<ResponseResult> Handle(ResumeSessionRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if (userInfo.userPassword == request.userPassword)
            {
                _signinLogsCommand.ClearTracking();
                var sessionLog = _signinLogsQuery.TableNoTracking.Where(x => x.token == userInfo.token).FirstOrDefault();
                sessionLog.lastActionTime = DateTime.Now;
                sessionLog.isLocked = false;
                await _signinLogsCommand.UpdateAsyn(sessionLog);
                return new ResponseResult()
                {
                    Result = Result.Success
                };
            }
            else
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "خطأ كلمة المرور",
                    ErrorMessageEn = "Password is wrong"
                };
        }
    }
}
