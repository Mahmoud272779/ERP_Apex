using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Application.Services.Process.Invoices;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.Invoices.POSTouch
{
    public class POSTouchController : ApiStoreControllerBase
    {
        private readonly IPOSTouchService _POSTouchService;

        public POSTouchController(IPOSTouchService _POSTouchService,
                      IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this._POSTouchService = _POSTouchService;
        }

        [HttpGet(nameof(getCategoriesOfPOS))]
        public async Task<ResponseResult> getCategoriesOfPOS()
        {
            var result = await _POSTouchService.getCategoriesOfPOS();
            return result;
        }
        [HttpGet(nameof(getItemsOfPOS))]
        public async Task<ResponseResult> getItemsOfPOS(int categoryId, int PageNumber, int PageSize ,string? itemName, int? PersonId, int? PriceListId)
        {
            var result = await _POSTouchService.getItemsOfPOS(categoryId,PageNumber,PageSize, itemName , PersonId , PriceListId);
            return result;
        }

        [HttpGet(nameof(GetPOSTouchSettings))]
        public async Task<ResponseResult> GetPOSTouchSettings()
        {
            var result = await _POSTouchService.GetPOSTouchSettings();
            return result;
        }

        [HttpPut(nameof(UpdatePOSTouchSettings))]
        public async Task<ResponseResult> UpdatePOSTouchSettings(POSTouchRequest request)
        {
            var result = await _POSTouchService.UpdatePOSTouchSettings(request);
            return result;
        }

        [HttpGet(nameof(getItemsOfPOSIOS))]
        public async Task<ResponseResult> getItemsOfPOSIOS(int categoryId, string? itemName,int PageNumber,int lastId, int PageSize, int? PriceListId, int? PersonId)
        {
            var result = await _POSTouchService.getItemsOfPOSIOS(categoryId, itemName, PageNumber, lastId, PageSize,   PriceListId,  PersonId);
            return result;
        }

        [HttpPost(nameof(FillItemForPOSIOS))]
        public async Task<ResponseResult> FillItemForPOSIOS([FromBody] FillItemCardQueryRequest parm)
        {

            //var result = await generalAPIsService.FillItemCardQuery(request);
            var result = await _POSTouchService.FillItemForPOSIOS(parm);
            return result;
        }
    }
}
