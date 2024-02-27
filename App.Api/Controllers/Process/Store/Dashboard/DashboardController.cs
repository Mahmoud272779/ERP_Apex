using App.Api.Controllers.BaseController;
using App.Application.Handlers.Invoices.POS.GetPOSInvoiceById;
using App.Application.Helpers;
using App.Application.Services.Process.StoreServices.Dashboard;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.Dashboard
{
   /// <summary>
   /// اى حاجه خلال الفتره الحاليه هتيجى مع كل ريكوست من العرض اللى فوق ----اى حاجه خلال السنه الماليه هتيجى مره واحده اما بعد اللوجن ال realod
   /// </summary>
    public class DashboardController : ApiStoreControllerBase
    {
        private readonly IDashboardData _dashboard;
        public DashboardController(IActionResultResponseHandler responseHandler, IDashboardData dashboard) : base(responseHandler)
        {
            _dashboard = dashboard;
        }
        /// <summary>
        /// اجمالى فواتير المبيعات والمشتريات الفتره الفتره الحالية
        /// </summary>
        /// <param name="dateFrom"> تاريخ بدايه الفتره الحالية  </param>
        /// <param name="dateTo"> تاريخ نهاية الفتره الحالية</param>
        /// <returns></returns>
        [HttpGet("GetCurrentPeroidTotalsForInvoices")]
        public async Task<ResponseResult> GetCurrentPeroidTotalsForInvoices(DateTime dateFrom,DateTime dateTo)
        {
            var result = await _dashboard.GetCurrenTPeroidTotalsForInvoices(dateFrom, dateTo);

            return result;
        }

        /// <summary>
        /// اجمالى فواتير المبيعات والمشتريات خلا السنة المالية__دى مش هتيجى غير مره واحده بعد اللوجن او ال reload مش هتيجى مع العرض كل مره
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentYearTotalsForInvoices")]
        public async Task<ResponseResult> GetCurrentYearTotalsForInvoices()
        {
            var result = await _dashboard.GetCurrenTYearTotalsForInvoices();

            return result;
        }
        /// <summary>
        /// الدخل خلال الفتره الحالية
        /// </summary>
        /// <param name="dateFrom"> تاريخ بداية الفتره الحالية</param>
        /// <param name="dateTo"> تاريخ انتهاء الفتره الحالية</param>
        /// <returns></returns>
        [HttpGet("IncommingCurrentPeriod")]
        public async Task<ResponseResult> IncommingCurrentPeriod(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _dashboard.IncommingCurrentPeriod(dateFrom, dateTo);

            return result;
        }
        /// <summary>
        /// الرصيد الفعلى للخزائن والبنوك
        /// </summary>
        /// <returns></returns>
        [HttpGet("BalancesOfBanksAndSafes")]
        public async Task<ResponseResult> BalancesOfBanksAndSafes()
        {
            var result = await _dashboard.BalancesOfBanksAndSafes();

            return result;
        }
        /// <summary>
        /// حركة المبيعات خلال السنه المالية
        /// </summary>
        /// <returns></returns>
        [HttpGet("SalesTrensaction")]
        public async Task<ResponseResult> SalesTrensaction()
        {
            var result = await _dashboard.SalesTrensaction();

            return result;
        }
        /// <summary>
        /// حركة المبيعات والمشتريات خلال الفتره الحالية
        /// </summary>
        /// <param name="dateFrom"> تاريخ بداية القترة الحالية</param>
        /// <param name="dateTo"> تاريخ انتهاء الفتره الحالية</param>
        /// <returns></returns>
        [HttpGet("SalesPurchasesTrensaction")]
        public async Task<ResponseResult> SalesPurchasesTrensaction(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _dashboard.SalesPurchasesTrensaction(dateFrom, dateTo);

            return result;
        }
        /// <summary>
        /// حركة الايردات والمصروفات خلال الفتره الحالية
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [HttpGet("RevenuesExpensesTransaction")]
        public async Task<ResponseResult> RevenuesExpensesTransaction(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _dashboard.RevenuesExpensesTransaction(dateFrom, dateTo);

            return result;
        }
        /// <summary>
        /// التدفق المالى خلال السنة المالية
        /// </summary>
        /// <returns></returns>
        [HttpGet("FinancailFlow")]
        public async Task<ResponseResult> FinancailFlow()
        {
            var result = await _dashboard.FinancailFlow();

            return result;
        }
        /// <summary>
        /// احدث الفواتير والاصناف الاكثر مبيعا خلال الفتره الحالية
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [HttpGet("NewestInvoicesAndMostSoldItems")]
        public async Task<ResponseResult> NewestInvoicesAndMostSoldItems(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _dashboard.NewestInvoicesAndMostSoldItems(dateFrom, dateTo);

            return result;
        }
        /// <summary>
        /// مناديب البيع الاكثر مبيعا خلال الفترة الحالية
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [HttpGet("SalesMenWhoSoldMost")]
        public async Task<ResponseResult> SalesMenWhoSoldMost(DateTime dateFrom, DateTime dateTo)
        {
            var result = await _dashboard.SalesMenWhoSoldMost(dateFrom, dateTo);

            return result;
        }


    }
}
