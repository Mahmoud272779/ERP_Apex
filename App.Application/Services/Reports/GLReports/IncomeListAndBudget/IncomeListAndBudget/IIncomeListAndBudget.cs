using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IIncomeListAndBudget
    {
        Task<totalResponseResult> getAllDataIncomeinListAndBudgetById(IncomeListSearchParameter parametr, int ID);
        Task<totalResponseResult> getTopLevelIncomingListAndBudget(IncomeListSearchParameter parameter);
        double SumEndTermMerchandis(DateTime To);
        Task<WebReport> IncomingListReport(IncomeListSearchParameter parameter, string ids, exportType exportType, bool isArabic, int fileId = 0);
    }
}
