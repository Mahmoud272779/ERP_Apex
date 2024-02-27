using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System.Threading.Tasks;

namespace App.Application
{
    public interface iBranchsService
    {
        Task<ResponseResult> getBranches();
        Task<ResponseResult> GetBranchesForAllUsers(int pageNumber,int pageSize);
        Task<ResponseResult> updatedSelectedBranch(int branchId);
    }
}
