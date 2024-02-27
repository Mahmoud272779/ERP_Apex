using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;using System.Threading.Tasks;

namespace App.Api
{
    public class PurchaseOrderController : ApiStoreControllerBase
    {

        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iRPT_Sales _iRPT_Sales;
        public PurchaseOrderController(
                                IActionResultResponseHandler ResponseHandler,
                                iAuthorizationService iAuthorizationService,
                                IMediator mediator,
                                iRPT_Sales iRPT_Sales) : base(ResponseHandler)
        {

            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iRPT_Sales = iRPT_Sales;
        }

        [HttpPost(nameof(AddPurchaseOrder))]
        public async Task<IActionResult> AddPurchaseOrder([FromForm] AddPurchaseOrderRequest parameter)
        {
            try
            {

                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchaseOrder_Purchases, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);

                var InvoicesDetails_ = Request.Form["InvoiceDetails"];
                foreach (var item in InvoicesDetails_)
                {

                    var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                    parameter.InvoiceDetails.Add(resReport);
                }

                var result = await _mediator.Send(parameter);


                /*  var print = await _mediator.Send(new SalesPrintWithSaveRequest());

                  if (print && result.Result == Result.Success)
                  {
                      //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                      var printResponse = await _mediator.Send(new InvoicePrintRequest
                      {
                          invoiceId = result.Id.Value,
                          invoiceCode = result.Code.ToString(),
                          exportType = exportType.Print,
                          screenId = (int)SubFormsIds.Sales
                      });
                      return Ok(printResponse);
                  }*/
                return Ok(result);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpGet("GetPurchaseOrderById")]
        public async Task<ResponseResult> GetPurchaseOrderById(int InvoiceId, bool isCopy)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchaseOrder_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetSalesServiceById.GetInvoiceById(InvoiceId, isCopy);
            var result = await _mediator.Send(new GetPurchaseOrderByIdRequest { InvoiceId = InvoiceId, isCopy = isCopy });
            return result;
        }

        //[HttpGet("PriceOfferReport")]
        //public async Task<IActionResult> PriceOfferReport(int InvoiceId, bool isCopy , exportType exportType,bool isArabic)
        //{
        //    var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.PurchaseOrder_Sales, Opretion.Open);
        //    if (isAuthorized != null)
        //        return Ok(isAuthorized);
        //    //var result = await GetSalesServiceById.GetInvoiceById(InvoiceId, isCopy);
        //    var result = await _iRPT_Sales.PriceOfferReport(new GetPurchaseOrderByIdRequest { InvoiceId = InvoiceId, isCopy = isCopy },exportType,isArabic);
        //    return Ok();
        //}

        [HttpPost(nameof(GetAllPurchaseOrder))]
        public async Task<ResponseResult> GetAllPurchaseOrder(GetAllPurchaseOrderRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchaseOrder_Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            var result = await _mediator.Send(parameter);
            return result;
        }


        [HttpPut(nameof(UpdatePurchaseOrder))]
        public async Task<IActionResult> UpdatePurchaseOrder([FromForm] UpdatePurchaseOrderRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchaseOrder_Purchases, Opretion.Edit);
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

           /* var print = await _mediator.Send(new SalesPrintWithSaveRequest());

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                var printResponse = await _mediator.Send(new InvoicePrintRequest
                {
                    invoiceId = result.Id.Value,
                    invoiceCode = result.Code.ToString(),
                    exportType = exportType.Print,
                    screenId = (int)SubFormsIds.Sales
                });
                return Ok(printResponse);
            }*/
            return Ok(result);
        }

        [HttpDelete("DeletePurchaseOrder")]
        public async Task<ResponseResult> DeletePurchaseOrder([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchaseOrder_Purchases, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
    
            var result = await _mediator.Send(new DeletePurchaseOrderRequest { Ids = InvoiceIdsList });
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
