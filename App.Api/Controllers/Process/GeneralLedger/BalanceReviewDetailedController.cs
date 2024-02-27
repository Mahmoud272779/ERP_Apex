using App.Api.Controllers.BaseController;
using App.Application.Services.Process.BalanceReviewDetailed;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class BalanceReviewDetailedController : ApiGeneralLedgerControllerBase
    {
        private readonly IBalanceReviewDetailedBusiness balanceReviewBusiness;
        public BalanceReviewDetailedController(IBalanceReviewDetailedBusiness BalanceReviewReportBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            balanceReviewBusiness = BalanceReviewReportBusiness;
        }
        [HttpGet("GetParentsDetails")]
        
        public async Task<IRepositoryResult> GetParentsDetails(int PageSize, int PageNumber,int FinancialAcountId,int? TypeOfPeriod,int? Monthes,DateTime? Year)
        {
            BalanceReviewDetailedParameter parameters = new BalanceReviewDetailedParameter()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
            };
            parameters.Search.FinancialAcountId = FinancialAcountId;
            parameters.Search.TypeOfPeriod = TypeOfPeriod;
            parameters.Search.Monthes = Monthes;
            parameters.Search.Year = Year;
            var account = await balanceReviewBusiness.CallRootDetails(parameters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
    }
}
