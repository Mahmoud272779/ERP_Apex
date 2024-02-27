using App.Domain.Entities.POS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.GetMyCurrentPOSSession
{
    public class currentPOSsessionHandler : IRequestHandler<currentPOSsessionRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly iUserInformation _userInformation;

        public currentPOSsessionHandler(IRepositoryQuery<POSSession> pOSSessionQuery, Helpers.iUserInformation userInformation)
        {
            _POSSessionQuery = pOSSessionQuery;
            _userInformation = userInformation;
        }

        public async Task<ResponseResult> Handle(currentPOSsessionRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _userInformation.GetUserInformation();
            var currentSession = _POSSessionQuery.TableNoTracking.Where(x => x.employeeId == userInfo.employeeId && x.sessionStatus == (int)POSSessionStatus.active).OrderBy(x => x.Id).LastOrDefault();
            if (currentSession != null)
                return new ResponseResult
                {
                    Id = currentSession.Id,
                    Result = Result.Success
                };
            else
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Note = "there is no active session available"
                };
        }
    }
}
