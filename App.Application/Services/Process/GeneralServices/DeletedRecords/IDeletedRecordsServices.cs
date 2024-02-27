using App.Application.Handlers.GeneralAPIsHandler;

namespace App.Application.Services.Process.GeneralServices.DeletedRecords
{
    public interface IDeletedRecordsServices
    {
        Task<ResponseResult> GetDeletedRecordsByDate(DateTime date);
        Task<ResponseResult> SetDeletedRecord(List<int> Ids, int type);
    }
}
