using App.Application.Helpers;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.POS;
using App.Domain.Models.Response.POS;
using MediatR;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.checkUserSessionStatus
{
    public class checkUserSessionStatusHandler : IRequestHandler<checkUserSessionStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly ISecurityIntegrationService _securityIntegrationService;

        public checkUserSessionStatusHandler(IRepositoryQuery<POSSession> pOSSessionQuery, iUserInformation iUserInformation, ISecurityIntegrationService securityIntegrationService)
        {
            _POSSessionQuery = pOSSessionQuery;
            _iUserInformation = iUserInformation;
            _securityIntegrationService = securityIntegrationService;
        }

        public async Task<ResponseResult> Handle(checkUserSessionStatusRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            var companyPOSSessions = _POSSessionQuery.TableNoTracking;
            var activeSessions = companyPOSSessions.Where(x => x.sessionStatus != (int)POSSessionStatus.closed);
            if(activeSessions.Where(x=> x.employeeId != userInfo.employeeId).Count() >= companyInfo.AllowedNumberOfPOS && !companyInfo.isInfinityNumbersOfPOS)
            {
                return new ResponseResult
                {
                    Result = Result.Success,
                    Data = new checkUserSessionStatusRsponse 
                    { 
                        userPOSSessionStatus     = (int)userPOSSessionChecker.notAllowed,
                        messageAr                = "عدد نقاط البيع المتاح لشركتم مكتمل",
                        messageEn               = "Your POS session Limit is not allowed to open new session"
                    }
                };
            }
            var userSessions = activeSessions.Where(x => x.employeeId == userInfo.employeeId);
            if(userSessions == null)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageAr = "الجلسه غير موجوده",
                    ErrorMessageEn = ""
                };
            if (!userSessions.Any())
                return new ResponseResult
                {
                    Result = Result.Success,
                    Data = new checkUserSessionStatusRsponse
                    {
                        userPOSSessionStatus = (int)userPOSSessionChecker.start,
                        messageAr = "هل تريد  فتح جلسه؟",
                        messageEn = "Do you want to open new Session ?"
                    }
                };
            else if (userSessions.FirstOrDefault().sessionStatus == (int)POSSessionStatus.active)
                return new ResponseResult
                {
                    Result = Result.Success,
                    Data = new checkUserSessionStatusRsponse
                    {
                        userPOSSessionStatus = (int)userPOSSessionChecker.opened,
                        messageAr = "فتح الجلسه الحالية",
                        messageEn = "Open Current Session"
                    }
                };
            else if (userSessions.FirstOrDefault().sessionStatus == (int)POSSessionStatus.bining)
                return new ResponseResult
                {
                    Result = Result.Success,
                    Data = new checkUserSessionStatusRsponse
                    {
                        userPOSSessionStatus = (int)userPOSSessionChecker.resume,
                        messageAr = "هل تريد استكمال الجلسه ؟",
                        messageEn = "Do you want to Rusume Session ?"
                    }
                };

            return null;
        }
    }
}
