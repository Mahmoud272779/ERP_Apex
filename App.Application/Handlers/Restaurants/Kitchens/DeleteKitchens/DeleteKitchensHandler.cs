using App.Application.Handlers.Restaurants;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.Restaurants;
using App.Domain.Entities.Process.Store;
using MediatR;
using System.Threading;

namespace App.Application.Handlers
{
    public class DeleteKitchensHandler : IRequestHandler<DeleteKitchensRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<Kitchens> KitchensQuery;
        private readonly IRepositoryCommand<Kitchens> KitchensCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IDeletedRecordsServices _deletedRecords;

        public DeleteKitchensHandler(IRepositoryQuery<Kitchens> KitchensQuery,
            IRepositoryCommand<Kitchens> KitchensCommand,
            ISystemHistoryLogsService systemHistoryLogsService,
            IDeletedRecordsServices deletedRecords)
        {
            this.KitchensQuery = KitchensQuery;
            this.KitchensCommand = KitchensCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _deletedRecords = deletedRecords;
        }
        public async Task<ResponseResult> Handle(DeleteKitchensRequest parameter, CancellationToken cancellationToken)
        {
            var kitchens = KitchensQuery.FindAll(e => parameter.Ids.Contains(e.Id) && e.Id != 1).ToList();
            KitchensCommand.RemoveRange(kitchens);
            await KitchensCommand.SaveAsync();
            var Ids = kitchens.Select(a => a.Id);
            if (Ids.ToList().Count > 0)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addUnits);

            //Fill The DeletedRecordTable

            _deletedRecords.SetDeletedRecord(Ids.ToList(), 1);

            return new ResponseResult() { Data = Ids, Id = null, Result = Ids.ToList().Count > 0 ? Result.Success : Result.Failed };
        }
    }
}
