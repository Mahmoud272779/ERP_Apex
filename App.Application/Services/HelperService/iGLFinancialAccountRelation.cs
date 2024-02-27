using App.Domain.Models.Shared;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.HelperService
{
    public interface iGLFinancialAccountRelation
    {
        Task<ResponseResult> GLRelation(GLFinancialAccountRelation type, int FinancialAccountId, int[] branchId, string arabicName, string latinName);
        Task<ResponseResult> personsGLRelation(bool IsSupplier, int FinancialAccountId, int[] Branches, string arabicName, string latinName);
    }
}
