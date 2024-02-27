using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice;
using App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById;
using App.Application.Handlers.Invoices.sales;
using App.Application.Handlers.Invoices.sales.DeleteSales;
using App.Application.Handlers.Invoices.sales.GetAllSales;
using App.Application.Handlers.Invoices.sales.UpdateSales;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api
{
    public class OfferPricecontroller : ApiStoreControllerBase
    {

        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iRPT_Sales _iRPT_Sales;
        public OfferPricecontroller(
                                IActionResultResponseHandler ResponseHandler,
                                iAuthorizationService iAuthorizationService,
                                IMediator mediator,
                                iRPT_Sales iRPT_Sales) : base(ResponseHandler)
        {

            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iRPT_Sales = iRPT_Sales;
        }

        [HttpPost(nameof(AddOfferPrice))]
        public async Task<IActionResult> AddOfferPrice([FromForm] AddOfferPriceRequest parameter, [FromQuery] bool isArabic = true)
        {
            try
            {

                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);

                var InvoicesDetails_ = Request.Form["InvoiceDetails"];
                foreach (var item in InvoicesDetails_)
                {

                    var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                    parameter.InvoiceDetails.Add(resReport);
                }

                var result = await _mediator.Send(parameter);


                  var print = await _mediator.Send(new SalesPrintWithSaveRequest());

                  if (print && result.Result == Result.Success)
                  {
                      //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                      var printResponse = await _mediator.Send(new InvoicePrintRequest
                      {
                          invoiceId = result.Id.Value,
                          invoiceCode = result.Code.ToString(),
                          exportType = exportType.Print,
                          screenId = (int)SubFormsIds.offerPrice_Sales,
                          isPriceOffer=true,
                          isArabic = isArabic


                      });
                      return Ok(printResponse);
                  }
                return Ok(result);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpGet("GetOfferPriceById")]
        public async Task<ResponseResult> GetOfferPriceById(int InvoiceId, bool isCopy)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetSalesServiceById.GetInvoiceById(InvoiceId, isCopy);
            var result = await _mediator.Send(new GetOfferPriceByIdRequest { InvoiceId = InvoiceId, isCopy = isCopy });
            return result;
        }

        //[HttpGet("PriceOfferReport")]
        //public async Task<IActionResult> PriceOfferReport(int InvoiceId, bool isCopy , exportType exportType,bool isArabic)
        //{
        //    var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Open);
        //    if (isAuthorized != null)
        //        return Ok(isAuthorized);
        //    //var result = await GetSalesServiceById.GetInvoiceById(InvoiceId, isCopy);
        //    var result = await _iRPT_Sales.PriceOfferReport(new GetOfferPriceByIdRequest { InvoiceId = InvoiceId, isCopy = isCopy },exportType,isArabic);
        //    return Ok();
        //}

        [HttpPost(nameof(GetAllOfferPrice))]
        public async Task<ResponseResult> GetAllOfferPrice(GetAllOfferPriceRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await _mediator.Send(parameter);
            return result;
        }


        [HttpPut(nameof(UpdateOfferPrice))]
        public async Task<IActionResult> UpdateOfferPrice([FromForm] UpdateOfferPriceRequest parameter, [FromQuery] bool isArabic = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }

         

            //var result = await updateSalesService.UpdateSales(parameter);
            var result = await _mediator.Send(parameter);

            var print = await _mediator.Send(new SalesPrintWithSaveRequest());

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                var printResponse = await _mediator.Send(new InvoicePrintRequest
                {
                    invoiceId = result.Id.Value,
                    invoiceCode = result.Code.ToString(),
                    exportType = exportType.Print,
                    screenId = (int)SubFormsIds.offerPrice_Sales,
                    isPriceOffer=true,
                     isArabic = isArabic

                });
                return Ok(printResponse);
            }
            return Ok(result);
        }

        [HttpDelete("DeleteOfferPrice")]
        public async Task<ResponseResult> DeleteOfferPrice([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.offerPrice_Sales, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
    
            var result = await _mediator.Send(new DeleteOfferPriceRequest { Ids = InvoiceIdsList });
            return result;
        }

        
        //[HttpGet(nameof(GetAllInvoiceIds))]
        //public async Task<ResponseResult> GetAllInvoiceIds(int invoiceTypeId)
        //{

        //    var isAuthorized = await _iAuthorizationService.isAuthorized(mainFormCode: 7, 41, opretion: Opretion.Open);
        //    if (isAuthorized != null)
        //        return isAuthorized;
        //    var result = await GetInvoiceService.GetAllInvoiceIds(invoiceTypeId);
        //    return result;
        //}
    }
}
