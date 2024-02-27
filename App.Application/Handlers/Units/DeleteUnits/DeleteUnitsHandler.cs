using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.Store;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class DeleteKitchensHandler : IRequestHandler<DeleteKitchensRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IRepositoryCommand<InvStpUnits> UnitsRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IDeletedRecordsServices _deletedRecords;

        public DeleteKitchensHandler(IRepositoryQuery<InvStpUnits> unitsRepositoryQuery,
            IRepositoryCommand<InvStpUnits> unitsRepositoryCommand,
            ISystemHistoryLogsService systemHistoryLogsService,
            IDeletedRecordsServices deletedRecords)
        {
            UnitsRepositoryQuery = unitsRepositoryQuery;
            UnitsRepositoryCommand = unitsRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _deletedRecords = deletedRecords;
        }
        public async Task<ResponseResult> Handle(DeleteKitchensRequest parameter, CancellationToken cancellationToken)
        {
            var units = UnitsRepositoryQuery.FindAll(e => parameter.Ids.Contains(e.Id) && e.Id != 1 &&
                       !e.CardUnits.Select(a => a.UnitId).Contains(e.Id)).ToList();
            UnitsRepositoryCommand.RemoveRange(units);
            await UnitsRepositoryCommand.SaveAsync();
            var Ids = units.Select(a => a.Id);
            var CountOfDleetedRows = units.Count();
            if (Ids.ToList().Count > 0)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteUnits,CountOfDleetedRows);

            //Fill The DeletedRecordTable

            _deletedRecords.SetDeletedRecord(Ids.ToList(), 1);

            /*var listRecords = new List<DeletedRecords>();
            foreach (var item in Ids.ToList())
            {
                var deleted = new DeletedRecords
                {
                    Type = 1,
                    DTime = DateTime.Now,
                    RecordID = item
                };
                listRecords.Add(deleted);
            }
            _deletedRecordCommand.AddRangeAsync(listRecords);*/

            return new ResponseResult() { Data = Ids, Id = null, Result = Ids.ToList().Count > 0 ? Result.Success : Result.Failed };
        }
    }
}
