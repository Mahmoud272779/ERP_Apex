using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.Process.FinancialAccountCostCenters;
using App.Domain.Models.Security.Authentication.Request;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class FinancialAccountCostController : ApiGeneralLedgerControllerBase
    {
        private readonly IFinancialAccountCostCenterBusiness financialAccountCostBusiness;
        public FinancialAccountCostController(IFinancialAccountCostCenterBusiness FinancialAccountCostBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            financialAccountCostBusiness = FinancialAccountCostBusiness;
        }
        [HttpPost(nameof(CreateNewFinancialAccountCostCenter))]
        public async Task<IRepositoryResult> CreateNewFinancialAccountCostCenter([FromBody] FinancialCostParameter parameter)
        {
            var add = await financialAccountCostBusiness.AddFinancialAccountCostCenter(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpPost(nameof(CreateNewFinancialAccountForCostCenter))]
        public async Task<IRepositoryResult> CreateNewFinancialAccountForCostCenter([FromBody] FinancialForCostParameter parameter)
        {
            var add = await financialAccountCostBusiness.AddFinancialAccountsForCostCenter(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpGet("GetAllFinancialAccountCostCenter")]

        public async Task<IRepositoryResult> GetAllFinancialAccountCostCenter(int FinancialAccountId)
        {
            var account = await financialAccountCostBusiness.GetFinancialAccountCostCenter(FinancialAccountId);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllFinancialAccountForCostCenter")]
        public async Task<IRepositoryResult> GetAllFinancialAccountForCostCenter(int costCenterId ,int pageNumber, int pageSize )
        {
            var account = await financialAccountCostBusiness.GetFinancialAccountsForCostCenter(costCenterId,pageNumber,pageSize );
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetFinancialAccountsWhichNotFoundForCostCenter")]
        public async Task<IRepositoryResult> GetFinancialAccountsWhichNotFoundForCostCenter(int pageNumber , int pageSize, int costCenterId)
        {
            var account = await financialAccountCostBusiness.GetFinancialAccountsWhichNotFoundForCostCenter(pageNumber,pageSize, costCenterId);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
    
        [HttpDelete("DeleteFinancialAccountCostCenter")]
        public async Task<IActionResult> DeleteFinancialAccountCostCenter([FromBody] int[] FinancialAccountId,int CostCenterId)
        {
            FinancialCostsParameter parameter = new FinancialCostsParameter()
            {
                FinancialAccountId = FinancialAccountId,
                CostCenterId= CostCenterId
            };
            var res = await financialAccountCostBusiness.RemoveFinancialAccountCostCenter(parameter);
            if (res.Result == Result.Success)
                return Ok(res);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, res);


        }
    }
}
