using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices;
using App.Application.Handlers.Invoices.ReturnPurchases.AddReturnPurchase;
using App.Application.Handlers.Invoices.ReturnPurchases.GetAllReturnPurchaseService;
using App.Application.Handlers.Invoices.ReturnPurchases.GetPruchase;
using App.Application.Handlers.Purchases.GetPrintWithSave;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers
{
    public class ReturnPurchaseWithOutVatController : ApiStoreControllerBase
    {
        
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        public ReturnPurchaseWithOutVatController(
                                        iAuthorizationService iAuthorizationService,
                                        IActionResultResponseHandler ResponseHandler,
                                        IMediator mediator,
                                        iUserInformation iUserInformation,
                                        IPermissionsForPrint iPermmissionsForPrint) : base(ResponseHandler)
        {
           
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
        }


        [HttpGet("GetPruchaseWithOutVat")]
        public async Task<ResponseResult> GetPruchaseWithOutVat(string InvoiceCode)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.ReturnPurchasesWithoutVat, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetPurchaseByCodeService.GetPruchase(InvoiceCode);
            var result = await _mediator.Send(new GetPruchaseWithoutVatRequest
            {
                InvoiceCode = InvoiceCode
            });
            return result;
        }


        [HttpPost(nameof(AddReturnPurchaseWithOutVat))]
        public async Task<IActionResult> AddReturnPurchaseWithOutVat([FromForm] AddReturnPurchaseWithoutVatRequest Parameter,[FromQuery] bool isArabic=true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.ReturnPurchasesWithoutVat, Opretion.Add);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {
                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                Parameter.InvoiceDetails.Add(resReport);
            }

            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {
                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                Parameter.PaymentsMethods.Add(resReport);
            }



            //var result = await AddReturnPurchaseService.AddReturnPurchase(Parameter);
            var result = await _mediator.Send(Parameter);


            //var print = settingService.TableNoTracking.FirstOrDefault().Purchases_PrintWithSave;
            var print = await _mediator.Send(new GetPrintWithSaveRequest());
            var userInfo = await _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.PurchasesReturn_Purchases, result.Code.ToString(), exportType.Print);

                var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.ReturnPurchasesWithoutVat);
                if (permissions.IsPrint)
                {
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.ReturnPurchasesWithoutVat,
                            isArabic=isArabic

                    });
                    printResponse.ResultForPrint = printResponse.Result;
                    printResponse.Result = result.Result;
                    return Ok(printResponse);
                }
                else
                {
                    var reportsReponse = new ReportsReponse()
                    {
                        ResultForPrint = Result.UnAuthorized,
                        Result = result.Result

                    };
                    return Ok(reportsReponse);
                }
            }

            return Ok(result);
        }



        [HttpPost(nameof(GetAllReturnPurchaseWithoutVat))]
        public async Task<ResponseResult> GetAllReturnPurchaseWithoutVat(GetAllReturnPurchaseWithoutVatRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.ReturnPurchasesWithoutVat, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await getAllPurchasesService.GetAllPurchase(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }
    }
}
