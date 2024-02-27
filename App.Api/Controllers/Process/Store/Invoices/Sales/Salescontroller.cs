using App.Api.Controllers.BaseController;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices.sales;
using App.Application.Handlers.Invoices.sales.DeleteSales;
using App.Application.Handlers.Invoices.sales.GetAllSales;
using App.Application.Handlers.Invoices.sales.UpdateSales;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintPermission;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Response.General;
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
    public class Salescontroller : ApiStoreControllerBase
    {
        //private readonly IAddSalesService addSalesService;
        //private readonly IGetInvoiceByIdService GetSalesServiceById;
        //private readonly IGetAllSalesServices getAllSalesServices;
        //private readonly IUpdateSalesService updateSalesService;
        //private readonly IGetPOSInvoicesService GetInvoiceService;
        //private readonly IDeleteSalesService deleteSalesInvoice;
        //private readonly IRepositoryQuery<InvGeneralSettings> settingService;
        //private readonly IPrintResponseService iPrintResponseService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        public Salescontroller( //IAddSalesService addSalesService, 
                                //IGetInvoiceByIdService GetSalesServiceById,
                                //IGetAllSalesServices getAllSalesServices, 
                                //IUpdateSalesService updateSalesService,
                                //IGetPOSInvoicesService getInvoiceService,
                                //IDeleteSalesService deleteSalesInvoice, 
                                //IRepositoryQuery<InvGeneralSettings> settingService,
                                //IPrintResponseService iPrintResponseService,
                                IActionResultResponseHandler ResponseHandler,
                                iAuthorizationService iAuthorizationService,
                                IMediator mediator,
                                iUserInformation iUserInformation,
                                IPermissionsForPrint iPermmissionsForPrint) : base(ResponseHandler)
        {
            //this.addSalesService = addSalesService;
            //this.GetSalesServiceById = GetSalesServiceById;
            //this.getAllSalesServices = getAllSalesServices;
            //this.updateSalesService = updateSalesService;
            //this.deleteSalesInvoice = deleteSalesInvoice;
            //GetInvoiceService = getInvoiceService;
            //this.settingService = settingService;
            //this.iPrintResponseService = iPrintResponseService;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
        }

        [HttpPost(nameof(AddSales))]
        public async Task<IActionResult> AddSales([FromForm] AddSalesRequest parameter,[FromQuery] bool isArabic = true)
        {
            try
            {

                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Sales, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
                var req = Request;

                var InvoicesDetails_ = Request.Form["InvoiceDetails"];
                foreach (var item in InvoicesDetails_)
                {

                    var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                    parameter.InvoiceDetails.Add(resReport);
                }

                var PaymentsMethods_ = Request.Form["PaymentsMethods"];
                foreach (var item in PaymentsMethods_)
                {
                    var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                    parameter.PaymentsMethods.Add(resReport);
                }



                //var result = await addSalesService.AddSales(parameter);
                var result = await _mediator.Send(parameter);


                //var print = settingService.TableNoTracking.FirstOrDefault().Sales_PrintWithSave;
                var print = result.isPrint; //await _mediator.Send(new SalesPrintWithSaveRequest());
                //var userInfo = _iUserInformation.GetUserInformation();

                if (print && result.Result == Result.Success)
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(result.permissionListId, (int)SubFormsIds.Sales);
                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds.Sales,
                            isArabic = isArabic

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
            catch (Exception e)
            {

                throw;
            }

        }

        [HttpGet("GetSalesById")]
        public async Task<ResponseResult> GetSalesById(int InvoiceId, bool isCopy)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetSalesServiceById.GetInvoiceById(InvoiceId, isCopy);
            var result = await _mediator.Send(new GetSalesByIdRequest { InvoiceId = InvoiceId,isCopy = isCopy});
            return result;
        }

        [HttpPost(nameof(GetAllSales))]
        public async Task<ResponseResult> GetAllSales(GetAllSalesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Sales, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await getAllSalesServices.GetAllSales(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }


        [HttpPut(nameof(UpdateSales))]
        public async Task<IActionResult> UpdateSales([FromForm] UpdateSalesRequest parameter, [FromQuery] bool isArabic = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Sales, Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }

            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {

                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                parameter.PaymentsMethods.Add(resReport);
            }

            //var result = await updateSalesService.UpdateSales(parameter);
            var result = await _mediator.Send(parameter);

            var print = await _mediator.Send(new SalesPrintWithSaveRequest());
            var userInfo = await _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Sales, result.Code.ToString(), exportType.Print);
                var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.Sales);
                if (permissions.IsPrint)
                {
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.Sales,
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

        [HttpDelete("DeleteSales")]
        public async Task<ResponseResult> DeleteSales([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Sales, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            //SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            //{
            //    Ids = InvoiceIdsList
            //};
            //var result = await deleteSalesInvoice.DeleteSales(parameter);
            var result = await _mediator.Send(new DeleteSalesRequest { Ids = InvoiceIdsList });
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
