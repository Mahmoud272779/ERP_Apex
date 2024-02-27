using App.Application.Helpers.Service_helper.History;
using DocumentFormat.OpenXml.Spreadsheet;
using App.Domain.Entities.Process.Restaurants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Application.Handlers.Restaurants.Kitchens.UpdateKitchens;
using App.Domain.Models.Request.Restaurants;

namespace App.Application.Handlers
{
    public class UpdateKitchensHandler : IRequestHandler<UpdateKitchensRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<Kitchens> kitchenCommand;
        private readonly IRepositoryQuery<Kitchens> kitchenQuery;
        private readonly IHistory<KitchensHistory> history;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;

        public UpdateKitchensHandler(IRepositoryCommand<Kitchens> kitchenCommand
                                    ,IRepositoryQuery<Kitchens> kitchenQuery
                                    , IHistory<KitchensHistory> history
                                    , ISystemHistoryLogsService systemHistoryLogsService)
        {
            this.kitchenCommand = kitchenCommand;
            this.kitchenQuery = kitchenQuery;
            this.history = history;
            this.systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> Handle(UpdateKitchensRequest parameters, CancellationToken cancellationToken)
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

            var ArabicUnitsExist = await kitchenQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);

            if (ArabicUnitsExist != null)
                return new ResponseResult() { Data = null, Id = ArabicUnitsExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var EnglishUnitsExist = await kitchenQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);

            if (EnglishUnitsExist != null)
                return new ResponseResult() { Data = null, Id = EnglishUnitsExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };



            var data = await kitchenQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateKitchensParameter, Kitchens>(parameters, data);

            if (table.Id == 1)
                table.Status = (int)Status.Active;

            table.UTime = DateTime.Now; // Set Time

            await kitchenCommand.UpdateAsyn(table);

            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            if (data != null)
                await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUnits);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };
        }
    }
}
