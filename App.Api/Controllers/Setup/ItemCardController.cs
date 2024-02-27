using App.Api.Controllers.BaseController;
using App.Application.Handlers.Setup.ItemCard;
using App.Application.Handlers.Setup.ItemCard.Query;
using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.StoreServices.Invoices.ItemsCard;
using App.Application.Services.Process.Unit;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using static Dapper.SqlMapper;

namespace App.Api.Controllers.Setup
{
    [Route("api/Store/[controller]")]
    //[Route("api/[controller]")]
    [ApiController]
    public class ItemCardController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IItemCardService _iItemCardService;
        private readonly IprintFileService _printFileService;


        public ItemCardController(IMediator mediator, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery, iAuthorizationService iAuthorizationService, IItemCardService iItemCardService, IprintFileService printFileService) : base(mediator)
        {
            _mediator = mediator;
            this.invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
            _iAuthorizationService = iAuthorizationService;
            _iItemCardService = iItemCardService;
            _printFileService = printFileService;
        }

        /// <summary>
        /// IsSearchdata, isPrint , Ids Used Only For print,please ignore them
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllItems")]
        public async Task<ActionResult<ResponseResult>> GetAllItems([FromQuery]GetAllItemCardRequest parm)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Open);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            //GetAllItemCardRequest request = new GetAllItemCardRequest() { PageNumber = PageNumber, PageSize = PageSize, Name = Name };
            var data = await _mediator.Send(parm);
            Type myType = data.Data.GetType();
            return data;
        }
        /// <summary>
        /// 
        /// IsSearchData=true only when use search 
        /// IsSearchData=false only when request for specific  data and ids must contains at least one id
        /// </summary>

        [HttpGet("ItemsCardReport")]
        public async Task<IActionResult> ItemsCardPrint([FromQuery] GetAllItemCardRequest parm,exportType exportType,bool isArabic,int fileId)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Open);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            WebReport report = new WebReport();
             report = await _iItemCardService.ItemCardPrint(parm, exportType, isArabic,fileId);
            return Ok(_printFileService.PrintFile(report, "ItemCard",exportType));

        }
        //public async Task<InvGeneralSettings> GetSettings()
        //{
        //    var settings = await invGeneralSettingsRepositoryQuery.GetAsync(1);
        //    return settings;
        //}

        [HttpPost]
        [Route("ItemCardFilter")]
        public async Task<ActionResult<ResponseResult>> ItemCardFilter(ItemCardFilterRequest request)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Open);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            return await _mediator.Send(request);
        }

        
        [HttpGet]
        [Route("GetItemById")]
        public async Task<ActionResult<GetItemCardResponse>> GetItemById(int Id)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Open);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            return await _mediator.Send(new GetItemCardRequest(Id));
        }

        
        [HttpGet]
        [Route("ItemCount")]
        public async Task<ActionResult<int>> ItemCount(int Id)
        {
            return await _mediator.Send(new GetItemCountRequest());
        }

        
        [HttpPost]
        [Route("PostItemCard")]
        public async Task<ActionResult<ResponseResult>> PostItemCard([FromForm] AddItemRequest request)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Add);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            var result = new ResponseResult();
            //If the item is of type serial only one unit should be sent from the front end
            if (request.TypeId == (int)ItemTypes.Serial)
            {
                if (request.Units.Count > 1)
                {
                    return new ResponseResult() { Data = "Select only one unit" };
                }
            }
            var units = Request.Form["Units"];
            var settings = await invGeneralSettingsRepositoryQuery.FindAsync(e => e.Id == 1);
            foreach (var item in units)
            {
                var resReport = JsonConvert.DeserializeObject<ItemUnitVM>(item);
                //Validate prices

                if (CheckUnitPriceValidation(resReport, settings.Other_ZeroPricesInItems))
                {
                    request.Units.Add(resReport);
                }
                else
                {
                    if(resReport.PurchasePrice > resReport.SalePrice1)
                    {
                        return new ResponseResult() 
                        { 
                            Result = Result.Failed, 
                            Note = "Purchase Price are invalid",
                            ErrorMessageEn = "Purchases price can not be more than sales price",
                            ErrorMessageAr = "لا يمكن أن يكون سعر الشراء أكثر من سعر البيع"
                        };

                    }
                    return new ResponseResult() { Result = Result.Failed, Note = "Prices are invalid"};
                }

            }

            var stores = Request.Form["Stores"];
            foreach (var item in stores)
            {

                var resReport = JsonConvert.DeserializeObject<ItemStoreVM>(item);
                request.Stores.Add(resReport);
            }

            var itemParts = Request.Form["Parts"];
            foreach (var item in itemParts)
            {

                var resReport = JsonConvert.DeserializeObject<ItemPartVM>(item);
                request.Parts.Add(resReport);
            }

            //for (int i = 0; i < 5000; i++)
            //{
            //    request.ArabicName = (1 + i).ToString();
            //    i--;
            //}
            result = await _mediator.Send(request);
            return result;
        }


        private bool CheckUnitPriceValidation(ItemUnitVM resReport, bool other_ZeroPricesInItems)
        {
            if (!other_ZeroPricesInItems)
            {
                if (resReport.SalePrice1 >= resReport.SalePrice2
                    && resReport.SalePrice1 >= resReport.SalePrice3
                    && resReport.SalePrice1 >= resReport.SalePrice4)
                {
                    if (resReport.SalePrice2 >= resReport.SalePrice3
                    && resReport.SalePrice2 >= resReport.SalePrice4)
                    {
                        if (resReport.SalePrice3 >= resReport.SalePrice4 && resReport.PurchasePrice <= resReport.SalePrice4)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (resReport.SalePrice1 >= resReport.SalePrice2
                && resReport.SalePrice1 >= resReport.SalePrice3
                && resReport.SalePrice1 >= resReport.SalePrice4)
                {
                    if (resReport.SalePrice2 >= resReport.SalePrice3
                    && resReport.SalePrice2 >= resReport.SalePrice4)
                    {
                        if (resReport.SalePrice3 >= resReport.SalePrice4 || resReport.PurchasePrice == 0)
                        {
                            return true ;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        
        [HttpPut]
        [Route("PutItemCard")]
        public async Task<ActionResult<ResponseResult>> PutItemCard([FromForm] UpdateItemRequest request)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Edit);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            var units = Request.Form["Units"];
            var settings = await invGeneralSettingsRepositoryQuery.FindAsync(e => e.Id == 1);
            foreach (var item in units)
            {
                var resReport = JsonConvert.DeserializeObject<ItemUnitVM>(item);
                //Validate prices
                if (resReport.PurchasePrice > resReport.SalePrice1)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        Note = "Purchase Price are invalid",
                        ErrorMessageEn = "Purchases price can not be more than sales price",
                        ErrorMessageAr = "لا يمكن أن يكون سعر الشراء أكثر من سعر البيع"
                    };

                }
                if (CheckUnitPriceValidation(resReport, settings.Other_ZeroPricesInItems))
                {
                    request.Units.Add(resReport);
                }
                else
                {
                    
                    return new ResponseResult() { Result = Result.Failed, Note = "Prices are invalid" };
                }
            }

            var stores = Request.Form["Stores"];
            foreach (var item in stores)
            {

                var resReport = JsonConvert.DeserializeObject<ItemStoreVM>(item);
                request.Stores.Add(resReport);
            }


            var itemParts = Request.Form["Parts"];
            foreach (var item in itemParts)
            {
                var resReport = JsonConvert.DeserializeObject<ItemPartVM>(item);
                request.Parts.Add(resReport);
            }


            var itemSerials = Request.Form["ItemSerials"];
            foreach (var item in itemSerials)
            {
                var resReport = JsonConvert.DeserializeObject<ItemSerialVM>(item);
                request.ItemSerials.Add(resReport);
            }


            return await _mediator.Send(request);
        }

        
        [HttpDelete]
        [Route("DeleteItemCards")]
        public async Task<ActionResult<ResponseResult>> DeleteItemsCards(List<int> Id)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Delete);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            DeleteItemsRequest request = new DeleteItemsRequest() { Items = Id };
            return await _mediator.Send(request);
        }

        
        [HttpPut]
        [Route("ModifyListOfItemsStatus")]
        public async Task<ActionResult<ResponseResult>> ModifyListOfItemsStatus(ModifyListOfItemsStatusRequest request)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Edit);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            return await _mediator.Send(request);
        }



        
        [HttpGet]
        [Route("GetItemCardHistoary")]
        public async Task<ActionResult<ResponseResult>> GetItemCardHistoary(int Id)
        {
            var iAuthorization = await _iAuthorizationService.isAuthorized((int)MainFormsIds.MainData, (int)SubFormsIds.ItemCard_MainUnits, Opretion.Open);
            if (iAuthorization != null)
                return Ok(iAuthorization);
            return await _mediator.Send(new GetItemCardHistoryRequest(Id));
        }


        
        [HttpGet]
        [Route("CheckBarcode")]
        public async Task<ActionResult<ResponseResult>> CheckBarcode(string? Barcode, string? ItemCode, string? NationalBarcode, int? ItemId, int? UnitId)
        {
            return await _mediator.Send(new CheckBarcodeRequest(Barcode, ItemCode, NationalBarcode, ItemId, UnitId));
        }

        
        [HttpGet]
        [Route("CheckItemName")]
        public async Task<ActionResult<ResponseResult>> CheckItemName(string ItemName, int? ItemId)
        {
            return await _mediator.Send(new CheckItemNameRequest(ItemName, ItemId));
        }

        
        [HttpGet]
        [Route("CheckStore")]
        public async Task<ActionResult<ResponseResult>> CheckStore(int Code)
        {
            return await _mediator.Send(new CheckStoreRequest(Code));
        }

        
        [HttpGet]
        [Route("CheckItem")]
        public async Task<ActionResult<ResponseResult>> CheckItem(string Code)
        {
            return await _mediator.Send(new CheckItemRequest(Code));
        }
        
        [HttpGet]
        [Route("GetLatestItemCode")]
        public async Task<ActionResult<ResponseResult>> GetLatestItemCode()
        {
            var settings = await invGeneralSettingsRepositoryQuery.GetAsync(1);
            if (!settings.Other_ItemsAutoCoding)
            {
                return new ResponseResult() { Data = null, Note = Aliases.GeneralSettingsMessages.AutomaticCodingIsDisabled };
            }
            else
                return await _mediator.Send(new GetLatestItemCodeRequest());
        }

        [HttpPost("FillItemCardQuery")]
        [Authorize]
        public async Task<ResponseResult> FillItemCardQuery([FromBody] FillItemCardQueryRequest parm)
        {
            var res = await _mediator.Send(parm);
            return res;
        }

        [HttpGet("GetItemsByDate")]
        public async Task<ResponseResult> GetItemsByDate(DateTime date, int PageNumber, int PageSize = 10)
        {
            var items = await _mediator.Send(new GetItemsByDateRequest(date,PageNumber,PageSize));
            return items;

        }
    }
}
