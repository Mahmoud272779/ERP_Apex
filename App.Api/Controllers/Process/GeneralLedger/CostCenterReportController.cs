using App.Api.Controllers.BaseController;
using App.Application.Services.Process.CostCentersReport;
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
    public class CostCenterReportController : ApiGeneralLedgerControllerBase
    {
        private readonly ICostCentersReportBusiness costCentersReportBusiness;
        public CostCenterReportController(ICostCentersReportBusiness CostCentersReportBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            costCentersReportBusiness = CostCentersReportBusiness;
        }      
        [HttpGet("GetCostCenterReport")]
        public async Task<IRepositoryResult> GetCostCenterReport(int PageSize, int PageNumber, DateTime? From, DateTime? To, int CostCenterId)
        {
            PageParameterCostCenterReport parameters = new PageParameterCostCenterReport()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
            };
            parameters.Search.From = From;
            parameters.Search.To = To;
            parameters.Search.CostCenterId = CostCenterId;
            var account = await costCentersReportBusiness.CallRootCostCenter(parameters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
    }
}
