
using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices.ReturnSales.AddReturnSales;
using App.Application.Handlers.Invoices.ReturnSales.GetAllReturnSales;
using App.Application.Handlers.Invoices.ReturnSales.GetReturnSales;
using App.Application.Handlers.Invoices.sales;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintPermission;
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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers
{
    public class ReturnSalesController: ApiStoreControllerBase
    {
        //private readonly IGetSalesByCodeForReturn GetSalesByCodeServiceForReturn;
        //private readonly IAddReturnSalesService AddReturnSalesService;
        //private readonly IGetAllReturnSalesService getAllReturnSalessService;
        //private readonly IGetAllSalesServices getAllSalessService;
        //private readonly IRepositoryQuery<InvGeneralSettings> settingService;
        //private readonly IPrintResponseService iPrintResponseService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        public ReturnSalesController(   //IGetSalesByCodeForReturn GetSalesByCodeServiceForReturn,
                                        //IAddReturnSalesService AddReturnSalesService,
                                        //IGetAllReturnSalesService getAllReturnSalessService,
                                        //IGetAllSalesServices getAllSalessService,
                                        //IRepositoryQuery<InvGeneralSettings> settingService,
                                        //IPrintResponseService iPrintResponseService,
                                        iAuthorizationService iAuthorizationService,
                                        IActionResultResponseHandler ResponseHandler,
                                        IMediator mediator,
                                        iUserInformation iUserInformation,
                                        IPermissionsForPrint iPermmissionsForPrint) : base(ResponseHandler)
        {
            //this.GetSalesByCodeServiceForReturn = GetSalesByCodeServiceForReturn;
            //this.AddReturnSalesService = AddReturnSalesService;
            //this.getAllReturnSalessService = getAllReturnSalessService;
            //this.getAllSalessService = getAllSalessService;
            //this.settingService = settingService;
            //this.iPrintResponseService = iPrintResponseService;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
        }


        [HttpGet("GetReturnSales")]
        public async Task<ResponseResult> GetReturnSales(string InvoiceCode)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.SalesReturn_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetSalesByCodeServiceForReturn.GetReturnSalesByCode(InvoiceCode);
            var result = await _mediator.Send(new GetReturnSalesRequest { InvoiceCode = InvoiceCode });
            return result;
        }


        [HttpPost(nameof(AddReturnSales))]
        public async Task<IActionResult> AddReturnSales([FromForm] AddReturnSalesRequest Parameter,[FromQuery] bool isArabic=true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.SalesReturn_Sales, Opretion.Add);
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



            //var result = await AddReturnSalesService.AddReturnSales(Parameter);
            var result = await _mediator.Send(Parameter);
            //var print = settingService.TableNoTracking.FirstOrDefault().Sales_PrintWithSave;
            var print = await _mediator.Send(new SalesPrintWithSaveRequest());
            var userInfo = await _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.SalesReturn_Sales, result.Code.ToString(), exportType.Print);

                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.SalesReturn_Sales);
                if (permissions.IsPrint)
                {
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.SalesReturn_Sales,
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



        [HttpPost(nameof(GetAllReturnSalesService))]
        public async Task<ResponseResult> GetAllReturnSalesService(GetAllReturnSalesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.SalesReturn_Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await getAllReturnSalessService.GetAllReturnSales(parameter);
            //var result = await getAllSalessService.GetAllSales(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }
    }
}
