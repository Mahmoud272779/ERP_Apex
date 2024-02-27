using App.Application.Handlers.GeneralAPIsHandler.HomeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Dashboard
{
    public interface IDashboardData
    {
       

        Task<ResponseResult> GetCurrenTPeroidTotalsForInvoices(DateTime dateFrom, DateTime dateTo);
        Task<ResponseResult> GetCurrenTYearTotalsForInvoices();
        Task<ResponseResult> IncommingCurrentPeriod(DateTime dateFrom, DateTime dateTo);

        Task<ResponseResult> BalancesOfBanksAndSafes();
        Task<ResponseResult> SalesTrensaction();
        Task<ResponseResult> SalesPurchasesTrensaction(DateTime dateFrom, DateTime dateTo);
        Task<ResponseResult> RevenuesExpensesTransaction(DateTime dateFrom, DateTime dateTo);
        Task<ResponseResult> FinancailFlow();
        Task<ResponseResult> NewestInvoicesAndMostSoldItems(DateTime dateFrom, DateTime dateTo);
        Task<ResponseResult> SalesMenWhoSoldMost(DateTime dateFrom, DateTime dateTo);

    }
}
