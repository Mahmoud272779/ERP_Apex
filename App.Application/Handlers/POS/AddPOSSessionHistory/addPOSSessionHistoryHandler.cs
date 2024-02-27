using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.POS;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.POS.AddPOSSessionHistory
{
    public class addPOSSessionHistoryHandler : IRequestHandler<addPOSSessionHistoryRequest, bool>
    {
        private readonly IRepositoryCommand<POSSessionHistory> _POSSessionHistoryCommand;
        private readonly iUserInformation _iUserInformation;

        public addPOSSessionHistoryHandler(IRepositoryCommand<POSSessionHistory> pOSSessionHistoryCommand, iUserInformation iUserInformation)
        {
            _POSSessionHistoryCommand = pOSSessionHistoryCommand;
            _iUserInformation = iUserInformation;
        }

        public async Task<bool> Handle(addPOSSessionHistoryRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var addHistory = await _POSSessionHistoryCommand.AddAsync(new POSSessionHistory
            {
                actionAr = request.actionAr,
                actionEn = request.actionEn,
                BrowserName = userInfo.browserName.ToString(),
                employeesId = userInfo.employeeId,
                POSSessionId = request.POSSessionId,
                LastDate = DateTime.Now,
               isTechnicalSupport = userInfo.isTechincalSupport
            });
            return addHistory;

        }
    }
}
