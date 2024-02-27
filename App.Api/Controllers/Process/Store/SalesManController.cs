using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.Sales_Man;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Nist;
using static App.Domain.Models.Security.Authentication.Request.SharedRequestDTOs;

namespace App.Api.Controllers.Process
{
    public class SalesManController : ApiStoreControllerBase
    {
        private readonly ISalesManService SalesMan_Service;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;

        public SalesManController(ISalesManService _SalesManServoce, iAuthorizationService iAuthorizationService,
            IActionResultResponseHandler ResponseHandeler, IprintFileService printFileService) : base(ResponseHandeler)
        {
            SalesMan_Service = _SalesManServoce;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
        }


        [HttpPost(nameof(AddSalesMan))]
        public async Task<ResponseResult> AddSalesMan(SalesManRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales,(int)SubFormsIds.Salesmen_Sales,Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            var add = new ResponseResult();
            //for (int i = 0; i < 5000; i++)
            //{
            //    parameter.ArabicName =  i.ToString();
            //    parameter.LatinName = i.ToString();
            //}
            add = await SalesMan_Service.AddSalesMan(parameter);
            return add;
        }

        
        [HttpGet("GetListOfSalesMan")]
        public async Task<ResponseResult> GetListOfSalesMan([FromQuery] SalesManSearch parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Salesmen_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //SalesManSearch parameter = new SalesManSearch()
            //{
            //    PageNumber = PageNumber,
            //    PageSize = PageSize,
            //    BranchList = BranchList,
            //    Name = Name,
            //};
            var add = await SalesMan_Service.GetListOfSalesMan(parameter);
            return add;
        }
        [HttpGet("SalesManReport")]
        public async Task<IActionResult> SalesManReport([FromQuery] SalesManSearch parameter,string? ids,bool isSearchData,exportType exportType,bool isArabic)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Salesmen_Sales, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            WebReport report = new WebReport();
            report = await SalesMan_Service.SalesManReport(parameter,ids,isSearchData, exportType, isArabic);
            return Ok(_printFileService.PrintFile(report, "SalesMan", exportType));
        }

        [HttpPut(nameof(UpdateSalesMan))]
        public async Task<ResponseResult> UpdateSalesMan(UpdateSalesManRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Salesmen_Sales, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SalesMan_Service.UpdateSalesMan(parameter);
            return add;
        }

        
        [HttpDelete("DeleteSalesMan")]
        public async Task<ResponseResult> DeleteSalesMan([FromBody] int[] Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Salesmen_Sales, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            SharedRequestDTOs.Delete CodeList = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var add = await SalesMan_Service.DeleteSalesMan(CodeList);
            return add;
        }

        
        [HttpGet("GetSalesManHistory")]
        public async Task<ResponseResult> GetSalesManHistory(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Salesmen_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await SalesMan_Service.GetSalesManHistory(Id);
            return add;
        }

        
        [HttpGet("GetSalesManDropDown")]
        public async Task<ResponseResult> GetSalesManDropDown([FromQuery] getDropDownlist param)
        {
            var add = await SalesMan_Service.GetSalesManDropDown(param);
            return add;
        }
        


    }
}
