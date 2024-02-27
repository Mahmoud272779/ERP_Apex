using App.Application.Handlers.Units;

namespace App.Application.Services.Process.Unit
{
    public interface IUnitsService
    {
        Task<ResponseResult> AddUnit(AddUnitRequest parameter);
        Task<ResponseResult> GetListOfUnits(GetListOfUnitsRequest parameters);
        Task<ResponseResult> UpdateUnits(UpdateUnitsRequest parameters);
        Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters);
        Task<ResponseResult> DeleteUnits(DeleteKitchensRequest parameter);
        Task<ResponseResult> GetUnitHistory(int UnitId);
        Task<ResponseResult> GetUnitsDropDown();
        Task<ResponseResult> GetUnitsByDate(GetUnitsByDateRequest parameter);

    }
}
