using App.Application.Helpers.Service_helper.History;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class AddUnitHandler : IRequestHandler<AddUnitRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IRepositoryCommand<InvStpUnits> UnitsRepositoryCommand;
        private readonly IHistory<InvUnitsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        public AddUnitHandler(IRepositoryQuery<InvStpUnits> unitsRepositoryQuery, IRepositoryCommand<InvStpUnits> unitsRepositoryCommand, IHistory<InvUnitsHistory> history, ISystemHistoryLogsService systemHistoryLogsService)
        {
            UnitsRepositoryQuery = unitsRepositoryQuery;
            UnitsRepositoryCommand = unitsRepositoryCommand;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> Handle(AddUnitRequest parameter, CancellationToken cancellationToken)
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;
            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var ArabicUnitExist = await UnitsRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicUnitExist != null)
                return new ResponseResult() { Data = null, Id = ArabicUnitExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishUnitExist = await UnitsRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);

            if (EnglishUnitExist != null)
                return new ResponseResult() { Data = null, Id = EnglishUnitExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };
            var table = Mapping.Mapper.Map<UnitsParameter, InvStpUnits>(parameter);
            int NextCode = UnitsRepositoryQuery.GetMaxCode(e => e.Code) + 1;
            table.Code = NextCode;
            table.UTime = DateTime.Now; //Set Time
            UnitsRepositoryCommand.Add(table);
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.addUnits);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
        }
    }
}
