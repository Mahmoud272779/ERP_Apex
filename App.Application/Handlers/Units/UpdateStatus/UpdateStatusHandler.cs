using App.Application.Helpers.Service_helper.History;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class UpdateStatusHandler : IRequestHandler<UpdateStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IRepositoryCommand<InvStpUnits> UnitsRepositoryCommand;
        private readonly IHistory<InvUnitsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateStatusHandler(ISystemHistoryLogsService systemHistoryLogsService, IHistory<InvUnitsHistory> history, IRepositoryCommand<InvStpUnits> unitsRepositoryCommand, IRepositoryQuery<InvStpUnits> unitsRepositoryQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            UnitsRepositoryCommand = unitsRepositoryCommand;
            UnitsRepositoryQuery = unitsRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(UpdateStatusRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var Units = UnitsRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var UnitsList = Units.ToList();

            UnitsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                UnitsList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var rssult = await UnitsRepositoryCommand.UpdateAsyn(UnitsList);
            foreach (var Unit in UnitsList)
            {
                history.AddHistory(Unit.Id, Unit.LatinName, Unit.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUnits);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }
}
