using System.Linq;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Purchases.AddPurchases;
using App.Application.Handlers.Purchases.DeletePurchase;
using App.Application.Handlers.Purchases.GetAllPurchases;
using App.Application.Handlers.Purchases.GetEmailForSuppliers;
using App.Application.Handlers.Purchases.GetInvoiceForSuppliers;
using App.Application.Handlers.Purchases.GetPrintWithSave;
using App.Application.Handlers.Purchases.GetPurchaseById;
using App.Application.Handlers.Purchases.SendEmailForSuppliers;
using App.Application.Handlers.Purchases.UpdatePurchases;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Services.Process.Invoices.Purchase;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers
{
    public class PurchaseWithOutVatController : ApiStoreControllerBase
    { 
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
 
        private readonly Itestfiles testfilesService;
        public PurchaseWithOutVatController(
                                  iAuthorizationService iAuthorizationService,
                                  IActionResultResponseHandler ResponseHandler, IMediator mediator
, iUserInformation iUserInformation
, IPermissionsForPrint iPermmissionsForPrint ) : base(ResponseHandler)
        {
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
         }
        [HttpPost(nameof(testFiles))]
        public async Task<bool> testFiles([FromForm] IFormFile[] AttachedFile)
        {
            var result = await testfilesService.testFiles(AttachedFile);

            return true;
        }
        [HttpPost(nameof(AddPurchaseWithoutVat))]
        public async Task<IActionResult> AddPurchaseWithoutVat([FromForm] AddPurchases_WOVRequest parameter, [FromQuery]bool isArabic = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Add);
            if (isAuthorized != null)
                return Ok(isAuthorized);



            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {
                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }
            var InvPurchaseAdditionalCostsRelations_ = Request.Form["OtherAdditionList"];
            foreach (var item in InvPurchaseAdditionalCostsRelations_)
            {
                var resReport = JsonConvert.DeserializeObject<OtherAdditionList>(item);
                parameter.OtherAdditionList.Add(resReport);
            }
            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {
                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                parameter.PaymentsMethods.Add(resReport);
            }

            var result = new ResponseResult();

                result = await _mediator.Send(parameter);

              var print = result.isPrint;

            if (print && result.Result == Result.Success)
            {
                try
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(result.permissionListId/*userInfo.permissionListId*/, (int)SubFormsIds.PurchasesWithoutVat);

                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds. PurchasesWithoutVat,
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
                catch (Exception ex)
                {
                    return Ok(ex.Message);
                }
            }

            return Ok(result);
        }
        

        [HttpPost(nameof(GetAllPurchaseWithoutVat))]
        public async Task<ResponseResult> GetAllPurchaseWithoutVat(GetAllPurchase_WOVRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetAllPurchaseService.GetAllPurchase(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpPut(nameof(UpdatePurchaseWithoutVat))]
        public async Task<IActionResult> UpdatePurchaseWithoutVat([FromForm] updatePurchases_WOVRequest parameter, [FromQuery] bool isArabic = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }

            var InvPurchaseAdditionalCostsRelations_ = Request.Form["InvPurchaseAdditionalCostsRelations"];
            foreach (var item in InvPurchaseAdditionalCostsRelations_)
            {

                var resReport = JsonConvert.DeserializeObject<OtherAdditionList>(item);
                parameter.OtherAdditionList.Add(resReport);
            }

            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {

                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                parameter.PaymentsMethods.Add(resReport);
            }

            //var result = await UpdatePurchaseService.UpdatePurchase(parameter);
            var result = await _mediator.Send(parameter);

            var print = await _mediator.Send(new GetPrintWithSaveRequest());
            var userInfo = await _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Purchases, result.Code.ToString(), exportType.Print);
                var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.PurchasesWithoutVat);
                if (permissions.IsPrint)
                {
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.PurchasesWithoutVat,
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

        [HttpDelete("DeletePurchaseWithoutVat")]
        public async Task<ResponseResult> DeletePurchaseWithoutVat([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            //SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            //{
            //    Ids = InvoiceIdsList
            //};
            //var result = await DeletePurchaseService.DeletePurchase(parameter);
            var result = await _mediator.Send(new DeletePurchase_WOVRequest
            {
                Ids = InvoiceIdsList
            });
            return result;
        }

       
        [HttpGet("GetEmailForSuppliersWithoutVat")]
        public async Task<ResponseResult> GetEmailForSuppliersWithoutVat(int InvoiceId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await SendEmailPurchases.GetEmailForSuppliers(InvoiceId);
            var result = await _mediator.Send(new GetEmailForSuppliersRequest { InvoiceId = InvoiceId });
            return result;
        }

        [HttpPost(nameof(SendEmailForSuppliersWithoutVat))]
        public async Task<ResponseResult> SendEmailForSuppliersWithoutVat([FromForm] SendEmailForSuppliers_WOVRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await SendEmailPurchases.SendEmailForSuppliers(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpGet("GetInvoiceForSuppliersWithoutVat")]
        public async Task<ResponseResult> GetInvoiceForSuppliersWithoutVat(int InvoiceId)
        {
            //var result = await SendEmailPurchases.GetInvoiceForSuppliers(InvoiceId);
            var result = await _mediator.Send(new GetInvoiceForSuppliers_WOVRequest { InvoiceId = InvoiceId });
            return result;
        }

    }
}
