using App.Api.Controllers.BaseController;
using App.Application.Services.Process.GLServices.ReceiptBusiness.CompinedReceiptsService;
using App.Domain;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Shared;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.GeneralLedger
{
    public class CompinedReceiptsController : ApiGeneralLedgerControllerBase
    {
        private readonly ICompinedReceiptsService _compinedReceiptsService;
        public CompinedReceiptsController(ICompinedReceiptsService compinedReceiptsService,
            IActionResultResponseHandler responseHandler) : base(responseHandler)
        {
            _compinedReceiptsService = compinedReceiptsService;
        }

        [HttpPost(nameof(CreateCompinedReceipt))]
        public async Task<ActionResult> CreateCompinedReceipt([FromForm] CompinedRecieptsRequest parameter)
        {
            var reciepts = Request.Form["Reciepts"];
            foreach (var item in reciepts)
            {
                var res = JsonConvert.DeserializeObject<CompinedReciept>(item);
                parameter.Reciepts.Add(res);
            }

            var add = await _compinedReceiptsService.AddCompinedReceipt(parameter);

            if (add.Result == Result.Success)
                return Ok(add);
            return StatusCode(StatusCodes.Status422UnprocessableEntity, add);
        }

        [HttpGet("GetAllCompinedReceipts")]
        public async Task<ResponseResult> GetAllCompinedReceipts(int PageSize, int PageNumber, DateTime? dateFrom, DateTime? dateTo, string? Code, int SafeOrBankId, int RecieptsTypeId)
        {
            GetRecieptsData data = new GetRecieptsData()
            {
                SafeOrBankId = SafeOrBankId,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Code = Code,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ReceiptsType = RecieptsTypeId,
            };
            var res = await _compinedReceiptsService.GetAllCompinedReciepts(data);
            return res;

        }
        [HttpGet("GetCompinedRecieptById")]
        public async Task<ResponseResult> GetCompinedRecieptById(int Id)
        {
            var res = await _compinedReceiptsService.GetCompinedRecieptById(Id);
            return res;
        }

    
        [HttpPut(nameof(UpdateCompinedReceipt))]
        public async Task<ActionResult> UpdateCompinedReceipt([FromForm] UpdateCompinedRecieptsRequest parameter)
        {
            var reciepts = Request.Form["Reciepts"];
            foreach (var item in reciepts)
            {
                var res = JsonConvert.DeserializeObject<CompinedReciept>(item);
                parameter.Reciepts.Add(res);
            }

            var add = await _compinedReceiptsService.UpdateCompinedReceipt(parameter);

            if (add.Result == Result.Success)
                return Ok(add);
            return StatusCode(StatusCodes.Status422UnprocessableEntity, add);
        }

        [HttpDelete("DeleteCompinedReciepts")]
        public async Task<IActionResult> DeleteCompinedReciepts(List<int> reciepts)
        {

            var isDEleted = await _compinedReceiptsService.DeleteCompinedReciept(reciepts);

            if (isDEleted.Result == Result.Success)
                return Ok(isDEleted);

            return StatusCode(StatusCodes.Status422UnprocessableEntity, isDEleted);
        }
    }
}
