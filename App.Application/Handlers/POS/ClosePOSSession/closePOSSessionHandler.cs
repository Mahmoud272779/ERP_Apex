using App.Application.Handlers.POS.AddPOSSessionHistory;
using App.Application.SignalRHub;
using App.Domain.Entities.POS;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.POS.ClosePOSSession
{
    public class closePOSSessionHandler : IRequestHandler<closePOSSessionRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<POSSession> _pOSSessionQuery;
        private readonly IRepositoryCommand<POSSession> _pOSSessionCommand;
        private readonly IHttpContextAccessor _httpContext;
        private readonly iUserInformation _iUserInformation;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMemoryCache _memoryCache;
        private readonly IMediator _mediator;

        public closePOSSessionHandler(IRepositoryQuery<POSSession> POSSessionQuery,
                                      IRepositoryCommand<POSSession> POSSessionCommand,
                                      IHttpContextAccessor httpContext,
                                      iUserInformation iUserInformation,
                                      IHubContext<NotificationHub> hub,
                                      IMemoryCache memoryCache,
                                      IMediator mediator)
        {
            _pOSSessionQuery = POSSessionQuery;
            _pOSSessionCommand = POSSessionCommand;
            _httpContext = httpContext;
            _iUserInformation = iUserInformation;
            _hub = hub;
            _memoryCache = memoryCache;
            _mediator = mediator;
        }
        public async Task<ResponseResult> Handle(closePOSSessionRequest request, CancellationToken cancellationToken)
        {
            //check is session exist
            var findSession = _pOSSessionQuery.TableNoTracking.Where(x => x.Id == request.sessionId).FirstOrDefault();
            if (findSession == null)
            {
                return new ResponseResult()
                {
                    Note = "Session Not Exist",
                    Result = Result.Failed
                };
            }
            var userInfo = await _iUserInformation.GetUserInformation();
            if (userInfo.employeeId != findSession.employeeId)
                if (!userInfo.otherSettings.allowCloseCloudPOSSession)
                    return new ResponseResult()
                    {
                        Note = "You can not close another person session",
                        Result = Result.Failed
                    };
            findSession.end = DateTime.Now;
            findSession.sessionClosedById = userInfo.employeeId;
            findSession.sessionStatus = (int)POSSessionStatus.closed;
            var updated = await _pOSSessionCommand.UpdateAsyn(findSession);
            if (updated)
            {
                await _mediator.Send(new addPOSSessionHistoryRequest
                {
                    POSSessionId = findSession.Id,
                    actionAr = "اغلاق الجلسة",
                    actionEn = "Close Session"
                });
                if (userInfo.employeeId != findSession.employeeId)
                {
                    //var clientConnectionId = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey).Where(x => x.EmployeeId == findSession.employeeId).FirstOrDefault();
                    //if(clientConnectionId != null)
                    //{
                    //    var singlRResponseMessage = new ResponseResult()
                    //    {
                    //        Note = "Your POS Session Is Closed",
                    //        ErrorMessageAr = $"لقد تم اخراجك من جلسه نقاط البيع عن طريق {userInfo.employeeNameAr}",
                    //        ErrorMessageEn = $"You kicked out of the POS Session By {userInfo.employeeNameEn}"
                    //    };
                    //    //await _hub.Clients.Clients(clientConnectionId.connectionId).SendAsync(defultData.POSSessionClose, singlRResponseMessage);
                    //}
                }
            }
            return new ResponseResult()
            {
                Note = updated ? "session closed Success" : "Closing session Faild",
                Result = updated ? Result.Success: Result.Failed
            };
           
        }
    }
}
