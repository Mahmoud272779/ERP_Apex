using App.Application.Handlers.POS.AddPOSSessionHistory;
using App.Application.Helpers;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.POS;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.OpenPOSSeassons
{
    public class openPOSSessionHandler : IRequestHandler<openPOSSessionRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSession> _pOSSessionQuery;
        private readonly IRepositoryCommand<POSSession> _pOSSessionCommand;
        private readonly IHttpContextAccessor _httpContext;
        private readonly iUserInformation _iUserInformation;
        private readonly ISecurityIntegrationService _ISecurityIntegrationService;
        private readonly IMediator _mediator;

        public openPOSSessionHandler(IRepositoryQuery<POSSession> POSSessionQuery, IRepositoryCommand<POSSession> POSSessionCommand, IHttpContextAccessor httpContext, iUserInformation iUserInformation, ISecurityIntegrationService iSecurityIntegrationService, IMediator mediator)
        {
            _pOSSessionQuery = POSSessionQuery;
            _pOSSessionCommand = POSSessionCommand;
            _httpContext = httpContext;
            _iUserInformation = iUserInformation;
            _ISecurityIntegrationService = iSecurityIntegrationService;
            _mediator = mediator;
        }
        public async Task<ResponseResult> Handle(openPOSSessionRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var companyInfo = await _ISecurityIntegrationService.getCompanyInformation();
            var _sessions = _pOSSessionQuery.TableNoTracking;
            var sessions = _sessions.Where(x => x.sessionStatus != (int)POSSessionStatus.closed);
            var Nextcode = _sessions.Any() ? _sessions.OrderBy(x => x.sessionCode).LastOrDefault().sessionCode + 1 : 1;
            var currentEmployeeSession = sessions.Where(x => x.employeeId == userInfo.employeeId).FirstOrDefault();
            if (currentEmployeeSession != null)
            {
                if (currentEmployeeSession.sessionStatus == (int)POSSessionStatus.active)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Note = "User has session open already"
                    };
                }
                else if (currentEmployeeSession.sessionStatus == (int)POSSessionStatus.bining)
                {
                    currentEmployeeSession.sessionStatus = (int)POSSessionStatus.active;
                    await _pOSSessionCommand.UpdateAsyn(currentEmployeeSession);
                    await _mediator.Send(new addPOSSessionHistoryRequest
                    {
                        POSSessionId = currentEmployeeSession.Id,
                        actionAr = "استكمال الجلسه",
                        actionEn = "Resume Session"
                    });
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Note = "User has Resume session"
                    };
                }
            }
            else
            {
                if (!companyInfo.isInfinityNumbersOfPOS)
                    if (sessions.Count() >= companyInfo.AllowedNumberOfPOS)
                        return new ResponseResult()
                        {
                            Note = "There is no free session, you cant open a new session",
                            Result = Result.Failed,
                            ErrorMessageAr = "تم الوصول الي اقصي عدد من نقاط البيع الخاص بشركتم",
                            ErrorMessageEn = "You have the maxmum POS sessions for your company"
                        };

                //var Nextcode = sessions.Any()? _sessions.OrderBy(x => x.sessionCode).LastOrDefault().sessionCode + 1 : 1;
                var newSession = new POSSession()
                {
                    employeeId = userInfo.employeeId,
                    start = DateTime.Now,
                    sessionStatus = (int)POSSessionStatus.active,
                    sessionCode = Nextcode
                };
                var added = await _pOSSessionCommand.AddAsync(newSession);
                if (added)
                    await _mediator.Send(new addPOSSessionHistoryRequest
                    {
                        POSSessionId = newSession.Id,
                        actionAr = "إنشاء جلسه",
                        actionEn = "Start new session"
                    });
                return new ResponseResult()
                {
                    Result = Result.Success,
                    Note = "Start Seasson Success"
                };
            }
            return new ResponseResult()
            {
                Result = Result.Failed,
                Note = ""
            };
        }
    }
}
