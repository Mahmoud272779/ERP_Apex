using App.Application.Helpers.Service_helper.History;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class UpdateUnitsHandler : IRequestHandler<UpdateUnitsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IRepositoryCommand<InvStpUnits> UnitsRepositoryCommand;
        private readonly IHistory<InvUnitsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateUnitsHandler(ISystemHistoryLogsService systemHistoryLogsService, IHistory<InvUnitsHistory> history, IRepositoryCommand<InvStpUnits> unitsRepositoryCommand, IRepositoryQuery<InvStpUnits> unitsRepositoryQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            UnitsRepositoryCommand = unitsRepositoryCommand;
            UnitsRepositoryQuery = unitsRepositoryQuery;
        }

        public async Task<ResponseResult> Handle(UpdateUnitsRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var ArabicUnitsExist = await UnitsRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);

            if (ArabicUnitsExist != null)
                return new ResponseResult() { Data = null, Id = ArabicUnitsExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishUnitsExist = await UnitsRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);

            if (EnglishUnitsExist != null)
                return new ResponseResult() { Data = null, Id = EnglishUnitsExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };



            var data = await UnitsRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateUnitsParameter, InvStpUnits>(parameters, data);

            if (table.Id == 1)
                table.Status = (int)Status.Active;

            table.UTime = DateTime.Now; // Set Time

            await UnitsRepositoryCommand.UpdateAsyn(table);

            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            if (data != null)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUnits);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };
        }
    }
}
