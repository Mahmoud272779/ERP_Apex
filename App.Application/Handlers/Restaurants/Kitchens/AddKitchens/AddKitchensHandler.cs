using App.Domain.Entities.Process.Restaurants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Models.Request.Restaurants;
using App.Application.Handlers.Restaurants.Kitchens;

namespace App.Application.Handlers
{
    public class AddKitchensHandler : IRequestHandler<AddKitchensRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<Kitchens> kitchenCommand;
        private readonly IRepositoryQuery<Kitchens> kitchenQuery;
        private readonly IHistory<KitchensHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public AddKitchensHandler(IRepositoryCommand<Kitchens> kitchenCommand
                                    , IRepositoryQuery<Kitchens> kitchenQuery
                                    , IHistory<KitchensHistory> history
                                    , ISystemHistoryLogsService systemHistoryLogsService)
        {
            this.kitchenCommand = kitchenCommand;
            this.kitchenQuery = kitchenQuery;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> Handle(AddKitchensRequest parameter, CancellationToken cancellationToken)
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
            var ArabicUnitExist = await kitchenQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicUnitExist != null)
                return new ResponseResult() { Data = null, Id = ArabicUnitExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishUnitExist = await kitchenQuery.GetByAsync(a => a.LatinName == parameter.LatinName);

            if (EnglishUnitExist != null)
                return new ResponseResult() { Data = null, Id = EnglishUnitExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };
            var table = Mapping.Mapper.Map<KitchensParameter,Kitchens>(parameter);
            int NextCode = kitchenQuery.GetMaxCode(e => e.Code) + 1;
            table.Code = NextCode;
            table.CanDelete = kitchenQuery.TableNoTracking.Count() == 0  ? false : true;
            table.UTime = DateTime.Now; //Set Time
            kitchenCommand.Add(table);
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.addUnits);
            
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
        }
    }
}
