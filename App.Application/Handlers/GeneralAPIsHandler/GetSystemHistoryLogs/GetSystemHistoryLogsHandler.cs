using App.Domain.Entities.Process.General;
using App.Domain.Models.Request.General;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralAPIsHandler.GetSystemHistoryLogs
{
    public class GetSystemHistoryLogsHandler : IRequestHandler<GetSystemHistoryLogsRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<SystemHistoryLogs> _systemHistoryLogsQuery;

        public GetSystemHistoryLogsHandler(iUserInformation iUserInformation, IRepositoryQuery<SystemHistoryLogs> systemHistoryLogsQuery)
        {
            _iUserInformation = iUserInformation;
            _systemHistoryLogsQuery = systemHistoryLogsQuery;
        }

        public async Task<ResponseResult> Handle(GetSystemHistoryLogsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var History = _systemHistoryLogsQuery.TableNoTracking
                .Include(x => x.employees)
                .Where(c => !c.isTechnicalSupport)
                //.Where(x => !userInfo.otherSettings.showDashboardForAllUsers && userInfo.userId != 1 ? x.employeesId == userInfo.employeeId : true)
                .Where(x => x.TransactionId != (int)SystemActionEnum.login ? x.BranchId == userInfo.CurrentbranchId : true)
                .Where(c => c.BranchId == userInfo.CurrentbranchId)
                .Where(c => c.date.Date >= request.dateFrom.Date && c.date.Date <= request.dateTo.Date)
                .Where(c => request.empId != 0 ? c.employeesId == request.empId : true)
                .OrderByDescending(x => x.Id)
                .Select(x => new HistoryMovement
                {
                    ArabicName = x.employees.ArabicName,
                    LatinName = x.employees.LatinName,
                    DateTime = x.date,
                    ArabicTransactionType = x.ActionArabicName,
                    LatinTransactionType = x.ActionLatinName
                });
            if(!request.isPrint)
                History = History.Skip((request.pageNumber - 1) * request.pageSize).Take(request.pageSize);
            return new ResponseResult()
            {
                Data = History,
                Result = Result.Success,
                DataCount = History.Count(),
                Note = request.pageNumber >= 5 ? "End Of Data" : ""
            };
        }
    }
}
