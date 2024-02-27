using System.Threading.Tasks;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;

namespace App.Application.Services.Process.Branches
{
    public interface IBranchesBusiness
    {
        Task<ResponseResult> AddBranch(BranchRequestsDTOs.Add parameter);
        Task<ResponseResult> UpdateBranch(BranchRequestsDTOs.Update parameter);
        Task<ResponseResult> GetBranchById(int Id);
        Task<ResponseResult> DeleteBranch(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllBranchData(BranchRequestsDTOs.Search paramters);
        Task<ResponseResult> GetAllBranchDataDropDown();
        Task<ResponseResult> GetAllBranchDataDropDownForPersons(bool isSuppler);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllBranchHistory(int BranchId);
        Task<ResponseResult> GetBranchesByDate(DateTime date, int PageNumber, int PageSize);
    }
}
