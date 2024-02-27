using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IBudgetReportService
    {
        Task<ResponseResult> getTopLevelBudget(IncomeListSearchParameter parametr);
        Task<ResponseResult> getAllDataBudgetById(IncomeListSearchParameter parametr,int Id);
        Task<WebReport> PublicBudgetReport(IncomeListSearchParameter parameter, string ids, exportType exportType, bool isArabic, int fileId = 0);
    }
}
