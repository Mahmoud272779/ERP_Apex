using App.Application.Handlers.Restaurants;
using App.Application.Helpers.Service_helper.History;
using App.Domain.Entities.Process.Restaurants;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers
{
    public class UpdateStatusHandler : IRequestHandler<UpdateStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<Kitchens> KitchensQuery;
        private readonly IRepositoryCommand<Kitchens> KitchensCommand;
        private readonly IHistory<KitchensHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateStatusHandler(ISystemHistoryLogsService systemHistoryLogsService
                                , IHistory<KitchensHistory> history
                                , IRepositoryCommand<Kitchens> KitchensCommand
                                , IRepositoryQuery<Kitchens> KitchensQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            this.KitchensCommand = KitchensCommand;
            this.KitchensQuery = KitchensQuery;
        }

        public async Task<ResponseResult> Handle(UpdateStatusRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var kitchens = KitchensQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var kitchensList = kitchens.ToList();

            kitchensList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                kitchensList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var result = await KitchensCommand.UpdateAsyn(kitchensList);
            foreach (var item in kitchensList)
            {
                history.AddHistory(item.Id, item.LatinName, item.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUnits);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }
}
