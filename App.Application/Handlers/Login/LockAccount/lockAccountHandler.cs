using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Login.LockAccount
{
    public class lockAccountHandler : IRequestHandler<lockAccountRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<signinLogs> _signinLogsQuery;
        private readonly IRepositoryCommand<signinLogs> _signinLogsCommand;
        public lockAccountHandler(iUserInformation iUserInformation, IRepositoryQuery<signinLogs> signinLogsQuery, IRepositoryCommand<signinLogs> signinLogsCommand)
        {
            _iUserInformation = iUserInformation;
            _signinLogsQuery = signinLogsQuery;
            _signinLogsCommand = signinLogsCommand;
        }
        public async Task<ResponseResult> Handle(lockAccountRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            _signinLogsCommand.ClearTracking();
            var sessionLog = _signinLogsQuery.TableNoTracking.Where(x => x.token == userInfo.token).FirstOrDefault();
            if (sessionLog != null)
            {
                sessionLog.isLocked = true;
                await _signinLogsCommand.UpdateAsyn(sessionLog);
                return new ResponseResult()
                {
                    Result = Result.Success
                };
            }
            else
            {
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = "Session is not exist"
                };
            }
        }
    }
}
