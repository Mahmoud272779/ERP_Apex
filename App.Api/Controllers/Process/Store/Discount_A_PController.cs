using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.Acquired_And_Premitted_Discount;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class Discount_A_PController : ApiStoreControllerBase
    {
        readonly private IDiscountService discountService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _iPrintFileService;


        public Discount_A_PController(IDiscountService _discountService, iAuthorizationService iAuthorizationService,
                     IActionResultResponseHandler ResponseHandler, IprintFileService iPrintFileService) : base(ResponseHandler)
        {
            discountService = _discountService;
            _iAuthorizationService = iAuthorizationService;
            _iPrintFileService = iPrintFileService;
        }


        [HttpPost(nameof(AddDiscount))]
        public async Task<ResponseResult> AddDiscount(DiscountRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)SubFormsIds.EarnedDiscount_Settelments: (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Add);
            if (isAuthorized != null)
                return isAuthorized;
            return await discountService.AddDiscount(parameter);
        }

        
        [HttpPut(nameof(UpdateDiscount))]
        public async Task<ResponseResult> UpdateDiscount(UpdateDiscountRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)SubFormsIds.EarnedDiscount_Settelments : (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Edit);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await discountService.UpdateDiscount(parameter);
            return add;
        }

        
        [HttpGet("GetListOfDiscounts")]

        public async Task<ResponseResult> GetListOfDiscounts([FromQuery] DiscountSearch parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Settelments, parameter.DocType == (int)DocumentType.AcquiredDiscount ? (int)SubFormsIds.EarnedDiscount_Settelments : (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var add = await discountService.GetListOfDiscounts(parameter);
            return add;
        }
        [HttpGet(nameof(getDiscountById))]
        public async Task<ResponseResult> getDiscountById([FromQuery]int Id, [FromQuery] int screenId)
        {
            int DocType = 0;
            if (screenId == (int)SubFormsIds.EarnedDiscount_Settelments)
                DocType = (int)DocumentType.AcquiredDiscount;
            else if(screenId == (int)SubFormsIds.PermittedDiscount_Settelments)
                DocType = (int)DocumentType.PermittedDiscount;

            var isAuthorized = await _iAuthorizationService.isAuthorized(0, DocType == (int)DocumentType.AcquiredDiscount ? (int)SubFormsIds.EarnedDiscount_Settelments : (int)SubFormsIds.PermittedDiscount_Settelments, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var res = await discountService.GetDiscountById(Id);
            return res;
        }
        [HttpGet("DiscountPrint")]
        public async Task<IActionResult> DiscountPrint(int documentId, int documentType,exportType exportType, bool isArabic, int fileId = 0)
        {
            WebReport report = new WebReport();
            report = await discountService.DiscountsReport(documentId, documentType, exportType,isArabic,fileId);
            string reportName = "";

            if (documentType == 14)
            {
               
                reportName = "EarnedDiscount";

            }
            else if (documentType == 15)
            {
                reportName = "PermittedDiscount";
            }
           

            return Ok(_iPrintFileService.PrintFile(report, reportName, exportType));
            //return Ok(file);

        }
       

        [HttpDelete("DeleteDiscount")]
        public async Task<ResponseResult> DeleteDiscount([FromBody] int[] Id)
        {
            SharedRequestDTOs.Delete ListCode = new SharedRequestDTOs.Delete()
            {
                Ids = Id
            };
            var add = await discountService.DeleteDiscount(ListCode);
            return add;
        }

        
        [HttpGet("GetDiscountHistory")]
        public async Task<ResponseResult> GetDiscountHistory(int Id)
        {
            var add = await discountService.GetDiscountHistory(Id);
            return add;
        }
    }
}
