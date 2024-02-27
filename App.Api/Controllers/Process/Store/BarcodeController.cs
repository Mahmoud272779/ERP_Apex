using System.Linq;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.BarCode;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.Barcode;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace App.Api.Controllers.Process
{
    public class BarcodeController: ApiStoreControllerBase
    {
        private readonly IBarCodeService BarCodeService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;
        private readonly IFilesMangerService _fileManager;
        public BarcodeController(IBarCodeService _BarCodeService, iAuthorizationService iAuthorizationService,
                                 IActionResultResponseHandler ResponseHandler,
                                 IprintFileService printFileService, IFilesMangerService fileManager) : base(ResponseHandler)
        {
            BarCodeService = _BarCodeService;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
            _fileManager = fileManager;
        }

        [HttpGet("FillItemCardBarcode")]
        public async Task<ResponseResult> FillItemCardBarcode(string? itemCode, int? unitId, int? itemId, int? itemType, int? categoryId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await BarCodeService.FillItemCardBarcode(itemCode, unitId , itemId, itemType , categoryId);
            return add;

        }
        
        [HttpPost(nameof(AddBarCode))]
        public async Task<ResponseResult> AddBarCode([FromForm]AddBarCodeRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var re = Request.Form["addBarCodeItemsRequests"];
            foreach (var item in re)  
            {

                var resReport = JsonConvert.DeserializeObject<AddBarCodeItemsRequest>(item);
                parameter.addBarCodeItemsRequests.Add(resReport);
            }
            var add = await BarCodeService.AddBarCode(parameter);
            return add;

        }
        
        [HttpPut(nameof(UpdateBarCode))]
        public async Task<ResponseResult> UpdateBarCode([FromForm] UpdateBarCodeRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var re = Request.Form["updateBarCodeItemsRequests"];
            foreach (var item in re)
            {
                var resReport = JsonConvert.DeserializeObject<UpdateBarCodeItemsRequest>(item);
                parameter.updateBarCodeItemsRequests.Add(resReport);
            }
            var add = await BarCodeService.updateBarCode(parameter);
            return add;

        }
        
        [HttpGet("GetAllBarCodes")]
        public async Task<ResponseResult> GetAllBarCodes(string? Name,int PageNumber,int PageSize)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            BarcodeSearch parameter = new BarcodeSearch()
            {
                Name = Name,
                PageNumber= PageNumber,
                PageSize= PageSize

            };
            var add = await BarCodeService.GetAllBarCode( parameter);
            return add;

        }
        
        [HttpGet("GetBarCodesById")]
        public async Task<ResponseResult> GetBarCodesById(int barcodeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await BarCodeService.GetBarCodeById(barcodeId);
            return add;

        }
        
        [HttpDelete("DeleteBarCodes")]
        public async Task<ResponseResult> DeleteBarCodes([FromBody] int[] BarCodeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete() 
            {
                Ids= BarCodeId
            };

            var add = await BarCodeService.DeleteBarCode(parameter);
            return add;

        }
        
        [HttpGet("GetBarCodesHistoryById")]
        public async Task<ResponseResult> GetBarCodesHistoryById(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await BarCodeService.GetAllBarCodeHistory(Id);
            return add;

        }
        
        [HttpGet("GetBarcodeDropDown")]
        public async Task<ResponseResult> GetBarcodeDropDown()
        {
            var add = await BarCodeService.GetBarcodeDropDown();
            return add;
        }
       
        
        [HttpPut(nameof(UpdateDefaultBarcode))]
        public async Task<ResponseResult> UpdateDefaultBarcode(DefaultBarCodeRequest barcodeId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await BarCodeService.UpdateDefaultBarcode(barcodeId);
            return add;
        }

        [HttpPut("PrintBarcode")]
        public async Task<IActionResult> PrintBarcode([FromBody] PrintItemsBarcodeRequest parameter, string? ids)
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Print);
            //if (isAuthorized != null)
            //    return Ok(isAuthorized);
            var permisionUser = await _fileManager.UserPermisionsForBarcodePrint();
            if (permisionUser.Result == Enums.Result.Success)
            {

                if (parameter.PrintItemsBarcodeRequestDetalies.Sum(c => c.count) > 1000)
                    return Ok(new ResponseResult
                    {
                        Result = Enums.Result.Failed,
                        ErrorMessageAr = "لا يمكن طباعه اكثر من 2000 استيكر",
                        ErrorMessageEn = "You can not print more than 2000 stickers"
                    });

                WebReport report = new WebReport();
                report = await BarCodeService.BarcodeReport(parameter);
                var res = await _printFileService.PrintFile(report, "barcode", exportType.Print,false,true);
                return Ok(res);
            }
            else return Ok(permisionUser);
            
        }

        [HttpGet("GetBarcodePrintFiles")]
        public async Task<ResponseResult> GetBarcodePrintFiles()
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Print);
            //if (isAuthorized != null)
            //    return Ok(isAuthorized);

           return  await _fileManager.BarcodePrintFiles();


        }

        [HttpGet("SetBarcodeDefaultFile")]
        public async Task<ResponseResult> SetBarcodeDefaultFile(int fileId)
        {
            //var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Repository, (int)SubFormsIds.Barcode_Repository, Opretion.Print);
            //if (isAuthorized != null)
            //    return Ok(isAuthorized);

            return await _fileManager.SetBarcodeDefautFile(fileId);


        }

    }
}
